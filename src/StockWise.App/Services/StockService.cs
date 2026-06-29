using Dapper;
using StockWise.App.Repositories;
using StockWise.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockWise.App.Services;

public class StockService : IStockService
{
    private readonly WarehouseRepository _warehouseRepo;
    private readonly DapperContext _dapper;

    public StockService(
        WarehouseRepository warehouseRepo,
        DapperContext dapper)
    {
        _warehouseRepo = warehouseRepo;
        _dapper = dapper;
    }

    public async Task<IEnumerable<StockItemDto>> GetStockByWarehouseAsync(int warehouseId)
    {
        using var connection = _dapper.CreateConnection();
        var rows = await connection.QueryAsync(
            @"SELECT sb.Id AS Id, sb.ItemId, i.Name, i.Article, i.Barcode,
                     sb.Quantity, sb.ReservedQty,
                     sb.Quantity - sb.ReservedQty AS AvailableQty,
                     sb.Price, sb.BatchNo, sb.ExpiryDate, i.MinStock
              FROM StockBalances sb
              INNER JOIN Items i ON i.Id = sb.ItemId
              WHERE sb.WarehouseId = @warehouseId AND i.IsBatch = 1

              UNION ALL

              SELECT MIN(sb.Id), sb.ItemId, i.Name, i.Article, i.Barcode,
                     SUM(sb.Quantity), SUM(sb.ReservedQty),
                     SUM(sb.Quantity - sb.ReservedQty),
                     AVG(sb.Price), CAST(NULL AS NVARCHAR(50)), CAST(NULL AS DATE), MIN(i.MinStock)
              FROM StockBalances sb
              INNER JOIN Items i ON i.Id = sb.ItemId
              WHERE sb.WarehouseId = @warehouseId AND i.IsBatch = 0
              GROUP BY sb.ItemId, i.Name, i.Article, i.Barcode, i.MinStock

              ORDER BY Name",
            new { warehouseId });

        return rows.Select(r =>
        {
            var expiry = r.ExpiryDate;
            DateOnly? expiryDate = expiry is DateTime dt
                ? DateOnly.FromDateTime(dt)
                : expiry is DateOnly d ? d : null;

            return new StockItemDto(
                (long)r.Id,
                (int)r.ItemId,
                (string)r.Name,
                (string?)r.Article,
                (string?)r.Barcode,
                (decimal)r.Quantity,
                (decimal)r.ReservedQty,
                (decimal)r.AvailableQty,
                (decimal)r.Price,
                (string?)r.BatchNo,
                expiryDate,
                (decimal)r.Quantity < (decimal)r.MinStock
            );
        });
    }

    public async Task<IEnumerable<WarehouseOptionDto>> GetWarehouseOptionsAsync()
    {
        var warehouses = await _warehouseRepo.GetAllAsync();
        return warehouses
            .Where(w => w.IsActive)
            .Select(w => new WarehouseOptionDto(w.Id, w.Name));
    }

    public async Task<WarehouseOptionDto?> GetDefaultWarehouseAsync()
    {
        var warehouses = await _warehouseRepo.GetAllAsync();
        var defaultW = warehouses.FirstOrDefault(w => w.IsActive);
        return defaultW is null ? null : new WarehouseOptionDto(defaultW.Id, defaultW.Name);
    }
}
