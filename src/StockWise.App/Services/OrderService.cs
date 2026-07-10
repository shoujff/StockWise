using Microsoft.EntityFrameworkCore;
using StockWise.App.Models;
using StockWise.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockWise.App.Services;

public class OrderService : IOrderService
{
    private readonly StockDb _db;
    private readonly IStockCoreService _stockCore;
    private readonly IItemService _itemService;

    public OrderService(StockDb db, IStockCoreService stockCore, IItemService itemService)
    {
        _db = db;
        _stockCore = stockCore;
        _itemService = itemService;
    }

    public async Task<IReadOnlyList<OrderListDto>> GetAllAsync(OrderFilterDto? filter = null)
    {
        var query = _db.Orders.AsQueryable();

        if (filter is not null)
        {
            if (!string.IsNullOrWhiteSpace(filter.Status))
                query = query.Where(o => o.Status == filter.Status);
            if (filter.DateFrom.HasValue)
                query = query.Where(o => o.CreatedAt >= filter.DateFrom.Value);
            if (filter.DateTo.HasValue)
                query = query.Where(o => o.CreatedAt <= filter.DateTo.Value);
            if (!string.IsNullOrWhiteSpace(filter.Search))
                query = query.Where(o => o.Number.Contains(filter.Search) ||
                    (o.Customer != null && o.Customer.Name.Contains(filter.Search)));
        }

        return await query
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderListDto(
                o.Id, o.Number, o.Status,
                o.Customer!.Name,
                o.TotalAmount, o.CreatedAt,
                o.Lines.Count,
                o.Lines.Sum(l => l.ShippedQty)))
            .ToListAsync();
    }

    public async Task<OrderDetailDto?> GetByIdAsync(int id)
    {
        return await _db.Orders
            .Include(o => o.Lines).ThenInclude(l => l.Item)
            .Include(o => o.Customer)
            .Where(o => o.Id == id)
            .Select(o => new OrderDetailDto(
                o.Id, o.Number, o.Status, o.TotalAmount, o.CreatedAt,
                o.Customer!.Name, o.CreatedBy,
                o.Lines.Select(l => new OrderLineDetailDto(
                    l.Id, l.ItemId, l.Item.Name, l.Item.Article, l.Item.Unit,
                    l.Quantity, l.Price, l.Amount, l.ShippedQty
                )).ToList()))
            .FirstOrDefaultAsync();
    }

    public async Task<OrderDetailDto> CreateAsync(CreateOrderDto dto, int userId)
    {
        if (dto.Lines.Count == 0)
            throw new ArgumentException("Заказ должен содержать хотя бы одну строку");

        var batchFlags = await GetItemBatchFlagsAsync(dto.Lines.Select(l => l.ItemId));

        Customer? customer = null;
        if (dto.CustomerId.HasValue && dto.CustomerId.Value > 0)
        {
            customer = await _db.Customers.FindAsync(dto.CustomerId.Value);
        }
        else if (!string.IsNullOrWhiteSpace(dto.CustomerName))
        {
            customer = await _db.Customers
                .FirstOrDefaultAsync(c => c.Name == dto.CustomerName);

            if (customer is null)
            {
                customer = new Customer { Name = dto.CustomerName };
                _db.Customers.Add(customer);
                await _db.SaveChangesAsync();
            }
        }

        var order = new Order
        {
            Number = await GenerateNumberAsync(),
            Status = "New",
            CustomerId = customer?.Id ?? 1,
            CreatedBy = userId
        };

        foreach (var line in dto.Lines)
        {
            var amount = line.Quantity * line.Price;
            order.Lines.Add(new OrderLine
            {
                ItemId = line.ItemId,
                Quantity = line.Quantity,
                Price = line.Price,
                Amount = amount,
                ShippedQty = 0
            });
            order.TotalAmount += amount;
        }

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        await ReserveStockAsync(order.Id, dto.Lines);

        return (await GetByIdAsync(order.Id))!;
    }

    private async Task ReserveStockAsync(int orderId, List<CreateOrderLineDto> lines)
    {
        foreach (var line in lines)
        {
            var balances = await _db.StockBalances
                .Where(sb => sb.ItemId == line.ItemId && sb.Quantity - sb.ReservedQty > 0)
                .OrderBy(sb => sb.ExpiryDate ?? DateOnly.MaxValue)
                .ThenBy(sb => sb.Id)
                .ToListAsync();

            var toReserve = line.Quantity;

            foreach (var balance in balances)
            {
                if (toReserve <= 0) break;

                var available = balance.AvailableQty;
                var take = Math.Min(available, toReserve);

                balance.ReservedQty += take;
                balance.UpdatedAt = DateTime.UtcNow;

                _db.Reservations.Add(new Reservation
                {
                    OrderId = orderId,
                    StockBalanceId = balance.Id,
                    Quantity = take,
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow
                });

                toReserve -= take;
            }

            if (toReserve > 0)
            {
                var item = await _db.Items.FindAsync(line.ItemId);
                var name = item?.Name ?? $"#{line.ItemId}";
                throw new InvalidOperationException(
                    $"Недостаточно доступного остатка для товара \"{name}\"");
            }
        }

        await _db.SaveChangesAsync();
    }

    public async Task ConfirmAsync(int id, int userId)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order is null)
            throw new ArgumentException("Заказ не найден");
        if (order.Status != "New")
            throw new InvalidOperationException(
                $"Нельзя подтвердить заказ со статусом \"{order.Status}\"");

        order.Status = "InProgress";
        await _db.SaveChangesAsync();
    }

    public async Task ShipAsync(int id, int userId)
    {
        var order = await _db.Orders
            .Include(o => o.Lines).ThenInclude(l => l.Item)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
            throw new ArgumentException("Заказ не найден");
        if (order.Status != "InProgress" && order.Status != "New")
            throw new InvalidOperationException(
                $"Нельзя отгрузить заказ со статусом \"{order.Status}\"");

        var reservations = await _db.Reservations
            .Where(r => r.OrderId == id && r.Status == "Active")
            .Include(r => r.StockBalance)
            .ToListAsync();

        if (reservations.Count == 0)
            throw new InvalidOperationException("Нет активных резервов для отгрузки");

        var byWarehouse = reservations
            .Where(r => r.StockBalance is not null)
            .GroupBy(r => r.StockBalance!.WarehouseId);

        foreach (var whGroup in byWarehouse)
        {
            var whId = whGroup.Key;
            var shipLines = new List<StockLineDto>();

            foreach (var reservation in whGroup)
            {
                var balance = reservation.StockBalance!;
                var line = order.Lines.FirstOrDefault(l => l.ItemId == balance.ItemId);
                if (line is null) continue;

                var remaining = line.Quantity - line.ShippedQty;
                if (remaining <= 0) continue;

                var take = Math.Min(reservation.Quantity, remaining);

                shipLines.Add(new StockLineDto(
                    line.ItemId, take, line.Price, balance.BatchNo, balance.ExpiryDate));

                reservation.Status = "Shipped";
                balance.ReservedQty -= take;
                balance.UpdatedAt = DateTime.UtcNow;

                line.ShippedQty += take;
            }

            if (shipLines.Count > 0)
            {
                var result = await _stockCore.PostOutcomeAsync(whId, shipLines, userId, "Order");
                if (!result.Success)
                    throw new InvalidOperationException(result.ErrorMessage ?? "Ошибка при отгрузке");
            }
        }

        if (order.Lines.All(l => l.ShippedQty >= l.Quantity))
            order.Status = "Shipped";

        await _db.SaveChangesAsync();
    }

    public async Task CancelAsync(int id)
    {
        var order = await _db.Orders
            .Include(o => o.Reservations)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
            throw new ArgumentException("Заказ не найден");
        if (order.Status is "Shipped" or "Cancelled")
            throw new InvalidOperationException(
                $"Нельзя отменить заказ со статусом \"{order.Status}\"");

        foreach (var reservation in order.Reservations.Where(r => r.Status == "Active"))
        {
            var balance = await _db.StockBalances.FindAsync(reservation.StockBalanceId);
            if (balance is not null)
            {
                balance.ReservedQty -= reservation.Quantity;
                balance.UpdatedAt = DateTime.UtcNow;
            }
            reservation.Status = "Released";
        }

        order.Status = "Cancelled";
        await _db.SaveChangesAsync();
    }

    public async Task<string> GenerateNumberAsync()
    {
        var prefix = "ORD";
        var year = DateTime.UtcNow.Year;
        var pattern = $"{prefix}-{year}-";

        var lastNumber = await _db.Orders
            .Where(o => o.Number.StartsWith(pattern))
            .OrderByDescending(o => o.Id)
            .Select(o => o.Number)
            .FirstOrDefaultAsync();

        int seq = 1;
        if (lastNumber is not null)
        {
            var parts = lastNumber.Split('-');
            if (parts.Length > 0 && int.TryParse(parts[^1], out var lastSeq))
                seq = lastSeq + 1;
        }

        return $"{pattern}{seq:D4}";
    }

    private async Task<Dictionary<int, bool>> GetItemBatchFlagsAsync(IEnumerable<int> itemIds)
    {
        var ids = itemIds.Distinct().ToList();
        var items = await _itemService.GetAllAsync();
        return items.Where(i => ids.Contains(i.Id)).ToDictionary(i => i.Id, i => i.IsBatch);
    }
}
