using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockWise.App.Services;

public record OrderListDto(
    int Id,
    string Number,
    string Status,
    string? CustomerName,
    decimal TotalAmount,
    DateTime CreatedAt,
    int LinesCount,
    decimal TotalShippedQty
);

public record OrderLineDetailDto(
    int Id,
    int ItemId,
    string? ItemName,
    string? Article,
    string? Unit,
    decimal Quantity,
    decimal Price,
    decimal Amount,
    decimal ShippedQty
);

public record OrderDetailDto(
    int Id,
    string Number,
    string Status,
    decimal TotalAmount,
    DateTime CreatedAt,
    string? CustomerName,
    int CreatedBy,
    List<OrderLineDetailDto> Lines
);

public record CreateOrderDto(
    int? CustomerId,
    string? CustomerName,
    List<CreateOrderLineDto> Lines
);

public record CreateOrderLineDto(
    int ItemId,
    decimal Quantity,
    decimal Price
);

public record OrderFilterDto(
    string? Status,
    DateTime? DateFrom,
    DateTime? DateTo,
    string? Search
);

public interface IOrderService
{
    Task<IReadOnlyList<OrderListDto>> GetAllAsync(OrderFilterDto? filter = null);
    Task<OrderDetailDto?> GetByIdAsync(int id);
    Task<OrderDetailDto> CreateAsync(CreateOrderDto dto, int userId);
    Task ConfirmAsync(int id, int userId);
    Task ShipAsync(int id, int userId);
    Task CancelAsync(int id);
    Task<string> GenerateNumberAsync();
}
