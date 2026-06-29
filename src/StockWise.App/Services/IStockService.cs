using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockWise.App.Services;

public record StockItemDto(
    long Id,
    int ItemId,
    string ItemName,
    string? Article,
    string? Barcode,
    decimal Quantity,
    decimal ReservedQty,
    decimal AvailableQty,
    decimal Price,
    string? BatchNo,
    DateOnly? ExpiryDate,
    bool IsLowStock
);

public record WarehouseOptionDto(
    int Id,
    string Name
);

public interface IStockService
{
    Task<IEnumerable<StockItemDto>> GetStockByWarehouseAsync(int warehouseId);
    Task<IEnumerable<WarehouseOptionDto>> GetWarehouseOptionsAsync();
    Task<WarehouseOptionDto?> GetDefaultWarehouseAsync();
}
