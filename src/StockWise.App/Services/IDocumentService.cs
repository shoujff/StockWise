using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockWise.App.Services;

public record DocumentListDto(
    int Id,
    string Type,
    string Number,
    DateTime Date,
    string Status,
    decimal TotalAmount,
    int LinesCount
);

public record DocumentDetailDto(
    int Id,
    string Type,
    string Number,
    DateTime Date,
    string Status,
    decimal TotalAmount,
    int? CustomerId,
    string? CustomerName,
    string? SupplierName,
    int? FromWarehouseId,
    string? FromWarehouseName,
    int? ToWarehouseId,
    string? ToWarehouseName,
    List<DocumentLineDto> Lines
);

public record DocumentLineDto(
    int Id,
    int ItemId,
    string? ItemName,
    string? Article,
    string? Unit,
    decimal Quantity,
    decimal Price,
    decimal Amount,
    string? BatchNo,
    DateOnly? ExpiryDate
);

public record CreateDocumentDto(
    string Type,
    int? CustomerId,
    string? SupplierName,
    int? FromWarehouseId,
    int? ToWarehouseId,
    List<CreateDocumentLineDto> Lines
);

public record CreateDocumentLineDto(
    int ItemId,
    decimal Quantity,
    decimal Price,
    string? BatchNo,
    DateOnly? ExpiryDate
);

public interface IDocumentService
{
    Task<IEnumerable<DocumentListDto>> GetAllAsync(string? typeFilter = null, string? statusFilter = null);
    Task<DocumentDetailDto?> GetByIdAsync(int id);
    Task<DocumentDetailDto> CreateAsync(CreateDocumentDto dto, int userId);
    Task<bool> DeleteAsync(int id);
    Task<(bool Success, string? Error)> PostAsync(int id, int userId);
    Task<(bool Success, string? Error)> CancelAsync(int id, int userId);
    Task<string> GenerateNumberAsync(string type);
}
