using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockWise.App.Services;

public record AbcAnalysisItemDto(
    int Id,
    string Name,
    string? Article,
    decimal TotalSales,
    decimal SharePct,
    decimal CumulativePct,
    string AbcClass
);

public record TurnoverReportDto(
    int ItemId,
    string ItemName,
    string? Article,
    int WarehouseId,
    string WarehouseName,
    decimal OpenBalance,
    decimal IncomeQty,
    decimal OutcomeQty,
    decimal CloseBalance
);

public record DashboardStatsDto(
    int TotalItems,
    decimal TotalStockValue,
    int DocumentsThisMonth,
    int LowStockItems,
    int ActiveWarehouses
);

public interface IReportService
{
    Task<IReadOnlyList<AbcAnalysisItemDto>> GetAbcAnalysisAsync(DateTime startDate, DateTime endDate);
    Task<IReadOnlyList<TurnoverReportDto>> GetTurnoverReportAsync(DateTime startDate, DateTime endDate, int? itemId = null);
    Task<DashboardStatsDto> GetDashboardStatsAsync();
}
