using Microsoft.EntityFrameworkCore;
using StockWise.App.Models;
using StockWise.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StockWise.App.Services;

public class StockCoreService : IStockCoreService
{
    private readonly StockDb _db;

    public StockCoreService(StockDb db)
    {
        _db = db;
    }

    public async Task<PostResult> PostIncomeAsync(
        int warehouseId,
        IEnumerable<StockLineDto> lines,
        int userId,
        string? refDocType = null)
    {
        var list = lines.ToList();
        if (list.Count == 0)
            return Fail("Нет строк для проводки");

        var refDocId = GenerateRefDocId();

        await using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            foreach (var line in list)
            {
                var balance = await FindOrCreateBalanceAsync(line.ItemId, warehouseId, line.BatchNo, line.ExpiryDate);

                balance.Quantity += line.Quantity;
                balance.Price = line.Price;
                balance.UpdatedAt = DateTime.UtcNow;

                _db.Transactions.Add(new Transaction
                {
                    Type = "Income",
                    Direction = "+",
                    Quantity = line.Quantity,
                    Price = line.Price,
                    BatchNo = line.BatchNo,
                    ExpiryDate = line.ExpiryDate,
                    ItemId = line.ItemId,
                    WarehouseId = warehouseId,
                    CreatedBy = userId,
                    RefDocId = refDocId,
                    RefDocType = refDocType ?? "Income"
                });
            }

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return Ok(refDocId);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return Fail($"Ошибка при проводке прихода: {ex.Message}");
        }
    }

    public async Task<PostResult> PostOutcomeAsync(
        int warehouseId,
        IEnumerable<StockLineDto> lines,
        int userId,
        string? refDocType = null)
    {
        var list = lines.ToList();
        if (list.Count == 0)
            return Fail("Нет строк для проводки");

        var refDocId = GenerateRefDocId();

        await using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            foreach (var line in list)
            {
                var balance = await FindBalanceForOutcomeAsync(line.ItemId, warehouseId, line.Quantity, line.BatchNo);
                if (balance is null)
                {
                    var item = await _db.Items.FindAsync(line.ItemId);
                    return Fail($"Недостаточно остатка для товара \"{item?.Name ?? "#" + line.ItemId}\" на складе");
                }

                balance.Quantity -= line.Quantity;
                balance.UpdatedAt = DateTime.UtcNow;

                _db.Transactions.Add(new Transaction
                {
                    Type = "Outcome",
                    Direction = "-",
                    Quantity = line.Quantity,
                    Price = line.Price,
                    BatchNo = line.BatchNo ?? balance.BatchNo,
                    ExpiryDate = line.ExpiryDate ?? balance.ExpiryDate,
                    ItemId = line.ItemId,
                    WarehouseId = warehouseId,
                    CreatedBy = userId,
                    RefDocId = refDocId,
                    RefDocType = refDocType ?? "Outcome"
                });
            }

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return Ok(refDocId);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return Fail($"Ошибка при проводке расхода: {ex.Message}");
        }
    }

    public async Task<PostResult> PostTransferAsync(
        int fromWarehouseId,
        int toWarehouseId,
        IEnumerable<StockLineDto> lines,
        int userId)
    {
        var list = lines.ToList();
        if (list.Count == 0)
            return Fail("Нет строк для проводки");

        var refDocId = GenerateRefDocId();

        await using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            foreach (var line in list)
            {
                var sourceBalance = await FindBalanceForOutcomeAsync(
                    line.ItemId, fromWarehouseId, line.Quantity, line.BatchNo);
                if (sourceBalance is null)
                {
                    var item = await _db.Items.FindAsync(line.ItemId);
                    return Fail($"Недостаточно остатка для товара \"{item?.Name ?? "#" + line.ItemId}\" на складе отправления");
                }

                sourceBalance.Quantity -= line.Quantity;
                sourceBalance.UpdatedAt = DateTime.UtcNow;

                _db.Transactions.Add(new Transaction
                {
                    Type = "Transfer",
                    Direction = "-",
                    Quantity = line.Quantity,
                    Price = line.Price,
                    BatchNo = line.BatchNo ?? sourceBalance.BatchNo,
                    ExpiryDate = line.ExpiryDate ?? sourceBalance.ExpiryDate,
                    ItemId = line.ItemId,
                    WarehouseId = fromWarehouseId,
                    CreatedBy = userId,
                    RefDocId = refDocId,
                    RefDocType = "Transfer"
                });

                var destBalance = await FindOrCreateBalanceAsync(
                    line.ItemId, toWarehouseId, line.BatchNo ?? sourceBalance.BatchNo,
                    line.ExpiryDate ?? sourceBalance.ExpiryDate);

                destBalance.Quantity += line.Quantity;
                destBalance.Price = line.Price;
                destBalance.UpdatedAt = DateTime.UtcNow;

                _db.Transactions.Add(new Transaction
                {
                    Type = "Transfer",
                    Direction = "+",
                    Quantity = line.Quantity,
                    Price = line.Price,
                    BatchNo = line.BatchNo ?? sourceBalance.BatchNo,
                    ExpiryDate = line.ExpiryDate ?? sourceBalance.ExpiryDate,
                    ItemId = line.ItemId,
                    WarehouseId = toWarehouseId,
                    CreatedBy = userId,
                    RefDocId = refDocId,
                    RefDocType = "Transfer"
                });
            }

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return Ok(refDocId);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return Fail($"Ошибка при перемещении: {ex.Message}");
        }
    }

    public async Task<PostResult> CancelDocumentAsync(string refDocType, int refDocId, int userId)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            var transactions = await _db.Transactions
                .Where(t => t.RefDocType == refDocType && t.RefDocId == refDocId)
                .ToListAsync();

            if (transactions.Count == 0)
                return Fail("Проводки для отмены не найдены — возможно, документ уже отменён");

            foreach (var t in transactions)
            {
                var reverseDirection = t.Direction == "+" ? "-" : "+";

                var balance = await _db.StockBalances
                    .FirstOrDefaultAsync(sb =>
                        sb.ItemId == t.ItemId &&
                        sb.WarehouseId == t.WarehouseId &&
                        (t.BatchNo == null || sb.BatchNo == t.BatchNo));

                if (balance is null)
                    return Fail("Остаток товара не найден на складе — возможно, он был удалён или перемещён");

                if (reverseDirection == "-" && balance.AvailableQty < t.Quantity)
                    return Fail("Недостаточно товара на складе для отмены — часть товара уже израсходована");

                if (reverseDirection == "+")
                    balance.Quantity += t.Quantity;
                else
                    balance.Quantity -= t.Quantity;

                balance.UpdatedAt = DateTime.UtcNow;

                _db.Transactions.Add(new Transaction
                {
                    Type = "Cancellation",
                    Direction = reverseDirection,
                    Quantity = t.Quantity,
                    Price = t.Price,
                    BatchNo = t.BatchNo,
                    ExpiryDate = t.ExpiryDate,
                    ItemId = t.ItemId,
                    WarehouseId = t.WarehouseId,
                    CreatedBy = userId,
                    RefDocId = refDocId,
                    RefDocType = refDocType
                });
            }

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return Ok(refDocId);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return Fail($"Ошибка при отмене документа: {ex.Message}");
        }
    }

    private async Task<StockBalance> FindOrCreateBalanceAsync(
        int itemId, int warehouseId, string? batchNo, DateOnly? expiryDate)
    {
        var balance = await _db.StockBalances
            .FirstOrDefaultAsync(sb =>
                sb.ItemId == itemId &&
                sb.WarehouseId == warehouseId &&
                (batchNo == null || sb.BatchNo == batchNo));

        if (balance is null)
        {
            balance = new StockBalance
            {
                ItemId = itemId,
                WarehouseId = warehouseId,
                BatchNo = batchNo,
                ExpiryDate = expiryDate,
                Quantity = 0,
                ReservedQty = 0,
                Price = 0,
                UpdatedAt = DateTime.UtcNow
            };
            _db.StockBalances.Add(balance);
        }

        return balance;
    }

    private async Task<StockBalance?> FindBalanceForOutcomeAsync(
        int itemId, int warehouseId, decimal requiredQty, string? preferredBatchNo)
    {
        if (!string.IsNullOrEmpty(preferredBatchNo))
        {
            return await _db.StockBalances
                .FirstOrDefaultAsync(sb =>
                    sb.ItemId == itemId &&
                    sb.WarehouseId == warehouseId &&
                    sb.BatchNo == preferredBatchNo &&
                    sb.Quantity - sb.ReservedQty >= requiredQty);
        }

            return await _db.StockBalances
            .Where(sb =>
                sb.ItemId == itemId &&
                sb.WarehouseId == warehouseId &&
                sb.Quantity - sb.ReservedQty >= requiredQty)
            .OrderBy(sb => sb.ExpiryDate ?? DateOnly.MaxValue)
            .ThenBy(sb => sb.Id)
            .FirstOrDefaultAsync();
    }

    private static int _lastRefDocId = Environment.TickCount;

    private static int GenerateRefDocId()
    {
        return Interlocked.Increment(ref _lastRefDocId);
    }

    private static PostResult Fail(string message) => new(false, message);
    private static PostResult Ok(int? refDocId = null) => new(true, null, refDocId);
}
