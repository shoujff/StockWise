using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockWise.App.Services;

public record InventoryListDto(
    int Id,
    string Number,
    string WarehouseName,
    DateTime Date,
    string Status,
    decimal TotalDiff,
    int LinesCount
);

public record InventoryDetailDto(
    int Id,
    string Number,
    string Status,
    DateTime Date,
    decimal TotalDiff,
    string WarehouseName,
    int CreatedBy,
    List<InventoryLineDto> Lines
);

public record InventoryLineDto(
    int Id,
    int ItemId,
    string? ItemName,
    string? Article,
    string? Unit,
    decimal ExpectedQty,
    decimal ActualQty,
    decimal Diff,
    decimal Price,
    string? BatchNo
);

public interface IInventoryService
{
    Task<IReadOnlyList<InventoryListDto>> GetAllAsync(string? statusFilter = null);
    Task<InventoryDetailDto?> GetByIdAsync(int id);
    Task<InventoryDetailDto> CreateAsync(int warehouseId, int userId);
    Task UpdateActualQtyAsync(int lineId, decimal actualQty);
    Task<(bool Success, string? Error)> ConfirmAsync(int id, int userId);
    Task<(bool Success, string? Error)> CancelAsync(int id);
    Task<string> GenerateNumberAsync();
}
