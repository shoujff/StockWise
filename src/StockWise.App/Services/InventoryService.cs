using Microsoft.EntityFrameworkCore;
using StockWise.App.Models;
using StockWise.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockWise.App.Services;

public class InventoryService : IInventoryService
{
    private readonly StockDb _db;
    private readonly IStockCoreService _stockCore;

    public InventoryService(StockDb db, IStockCoreService stockCore)
    {
        _db = db;
        _stockCore = stockCore;
    }

    public async Task<IReadOnlyList<InventoryListDto>> GetAllAsync(string? statusFilter = null)
    {
        var query = _db.Inventories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(statusFilter))
            query = query.Where(i => i.Status == statusFilter);

        return await query
            .OrderByDescending(i => i.Date)
            .Select(i => new InventoryListDto(
                i.Id, i.Number, i.Warehouse.Name, i.Date, i.Status, i.TotalDiff, i.Lines.Count))
            .ToListAsync();
    }

    public async Task<InventoryDetailDto?> GetByIdAsync(int id)
    {
        return await _db.Inventories
            .Include(i => i.Lines).ThenInclude(l => l.Item)
            .Include(i => i.Warehouse)
            .Where(i => i.Id == id)
            .Select(i => new InventoryDetailDto(
                i.Id, i.Number, i.Status, i.Date, i.TotalDiff,
                i.Warehouse.Name, i.CreatedBy,
                i.Lines.Select(l => new InventoryLineDto(
                    l.Id, l.ItemId, l.Item.Name, l.Item.Article, l.Item.Unit,
                    l.ExpectedQty, l.ActualQty, l.Diff, l.Price, l.BatchNo
                )).ToList()))
            .FirstOrDefaultAsync();
    }

    public async Task<InventoryDetailDto> CreateAsync(int warehouseId, int userId)
    {
        var warehouse = await _db.Warehouses.FindAsync(warehouseId);
        if (warehouse is null)
            throw new ArgumentException("Склад не найден");

        var balances = await _db.StockBalances
            .Where(sb => sb.WarehouseId == warehouseId)
            .Include(sb => sb.Item)
            .ToListAsync();

        if (balances.Count == 0)
            throw new ArgumentException("На складе нет остатков для инвентаризации");

        var inventory = new Inventory
        {
            Number = await GenerateNumberAsync(),
            WarehouseId = warehouseId,
            Date = DateTime.UtcNow,
            Status = "Draft",
            CreatedBy = userId,
            UserId = userId
        };

        foreach (var balance in balances)
        {
            inventory.Lines.Add(new InventoryLine
            {
                ItemId = balance.ItemId,
                ExpectedQty = balance.Quantity,
                ActualQty = balance.Quantity,
                Diff = 0,
                Price = balance.Price,
                BatchNo = balance.BatchNo
            });
        }

        _db.Inventories.Add(inventory);
        await _db.SaveChangesAsync();

        return (await GetByIdAsync(inventory.Id))!;
    }

    public async Task UpdateActualQtyAsync(int lineId, decimal actualQty)
    {
        var line = await _db.InventoryLines.FindAsync(lineId);
        if (line is null)
            throw new ArgumentException("Строка инвентаризации не найдена");

        line.ActualQty = actualQty;
        line.Diff = actualQty - line.ExpectedQty;
        await _db.SaveChangesAsync();
    }

    public async Task<(bool Success, string? Error)> ConfirmAsync(int id, int userId)
    {
        var inventory = await _db.Inventories
            .Include(i => i.Lines)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (inventory is null)
            return (false, "Инвентаризация не найдена");
        if (inventory.Status != "Draft")
            return (false, $"Нельзя подтвердить инвентаризацию со статусом \"{inventory.Status}\"");

        var diffs = inventory.Lines.Where(l => l.Diff != 0).ToList();
        inventory.TotalDiff = diffs.Sum(l => Math.Abs(l.Diff));

        foreach (var line in diffs)
        {
            var stockLine = new StockLineDto(
                line.ItemId, Math.Abs(line.Diff), line.Price, line.BatchNo, null);

            PostResult result;
            if (line.Diff > 0)
            {
                result = await _stockCore.PostIncomeAsync(
                    inventory.WarehouseId, [stockLine], userId, "Inventory");
            }
            else
            {
                result = await _stockCore.PostOutcomeAsync(
                    inventory.WarehouseId, [stockLine], userId, "Inventory");
            }

            if (!result.Success)
                return (false, result.ErrorMessage);
        }

        inventory.Status = "Confirmed";
        await _db.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> CancelAsync(int id)
    {
        var inventory = await _db.Inventories.FindAsync(id);
        if (inventory is null)
            return (false, "Инвентаризация не найдена");
        if (inventory.Status != "Draft")
            return (false, "Можно отменить только черновик инвентаризации");

        inventory.Status = "Cancelled";
        await _db.SaveChangesAsync();

        return (true, null);
    }

    public async Task<string> GenerateNumberAsync()
    {
        var prefix = "INV";
        var year = DateTime.UtcNow.Year;
        var pattern = $"{prefix}-{year}-";

        var lastNumber = await _db.Inventories
            .Where(i => i.Number.StartsWith(pattern))
            .OrderByDescending(i => i.Id)
            .Select(i => i.Number)
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
}
