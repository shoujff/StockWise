using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockWise.App.Services;

public record StockLineDto(
    int ItemId,
    decimal Quantity,
    decimal Price,
    string? BatchNo,
    DateOnly? ExpiryDate
);

public record PostResult(
    bool Success,
    string? ErrorMessage,
    int? RefDocId = null
);

public interface IStockCoreService
{
    Task<PostResult> PostIncomeAsync(int warehouseId, IEnumerable<StockLineDto> lines, int userId, string? refDocType = null);
    Task<PostResult> PostOutcomeAsync(int warehouseId, IEnumerable<StockLineDto> lines, int userId, string? refDocType = null);
    Task<PostResult> PostTransferAsync(int fromWarehouseId, int toWarehouseId, IEnumerable<StockLineDto> lines, int userId);
    Task<PostResult> CancelDocumentAsync(string refDocType, int refDocId, int userId);
}
