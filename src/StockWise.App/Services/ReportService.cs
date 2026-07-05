using Dapper;
using StockWise.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StockWise.App.Services;

public class ReportService : IReportService
{
    private readonly DapperContext _dapper;

    public ReportService(DapperContext dapper)
    {
        _dapper = dapper;
    }

    public async Task<IReadOnlyList<AbcAnalysisItemDto>> GetAbcAnalysisAsync(
        DateTime startDate, DateTime endDate)
    {
        using var connection = _dapper.CreateConnection();

        var rows = await connection.QueryAsync(
            @"WITH Sales AS (
                SELECT i.Id, i.Name, i.Article,
                       SUM(t.Quantity * t.Price) AS TotalSales
                FROM Transactions t
                JOIN Items i ON i.Id = t.ItemId
                WHERE t.Direction = '-' AND t.Type = 'Outcome'
                  AND t.CreatedAt >= @startDate AND t.CreatedAt < @endDate
                GROUP BY i.Id, i.Name, i.Article
            ),
            Ranked AS (
                SELECT Id, Name, Article, TotalSales,
                       TotalSales * 1.0 / NULLIF(SUM(TotalSales) OVER(), 0) AS SharePct,
                       SUM(TotalSales) OVER(ORDER BY TotalSales DESC, Id) /
                           NULLIF(SUM(TotalSales) OVER(), 0) AS CumulativePct
                FROM Sales
            )
            SELECT Id, Name, Article, TotalSales,
                   ROUND(SharePct * 100, 2) AS SharePct,
                   ROUND(CumulativePct * 100, 2) AS CumulativePct,
                   CASE
                       WHEN CumulativePct <= 0.80 THEN 'A'
                       WHEN CumulativePct <= 0.95 THEN 'B'
                       ELSE 'C'
                   END AS AbcClass
            FROM Ranked
            ORDER BY TotalSales DESC",
            new { startDate, endDate });

        return rows.Select(r => new AbcAnalysisItemDto(
            (int)r.Id,
            (string)r.Name,
            (string?)r.Article,
            (decimal)r.TotalSales,
            (decimal)r.SharePct,
            (decimal)r.CumulativePct,
            (string)r.AbcClass
        )).ToList();
    }

    public async Task<IReadOnlyList<TurnoverReportDto>> GetTurnoverReportAsync(
        DateTime startDate, DateTime endDate, int? itemId = null)
    {
        using var connection = _dapper.CreateConnection();

        var rows = await connection.QueryAsync(
            @"WITH OpenBalance AS (
                SELECT t.ItemId, t.WarehouseId,
                       SUM(CASE WHEN t.Direction = '+' THEN t.Quantity ELSE -t.Quantity END) AS OpenQty
                FROM Transactions t
                WHERE t.CreatedAt < @startDate
                GROUP BY t.ItemId, t.WarehouseId
            ),
            PeriodMovements AS (
                SELECT t.ItemId, t.WarehouseId,
                       SUM(CASE WHEN t.Direction = '+' THEN t.Quantity ELSE 0 END) AS IncomeQty,
                       SUM(CASE WHEN t.Direction = '-' THEN t.Quantity ELSE 0 END) AS OutcomeQty
                FROM Transactions t
                WHERE t.CreatedAt >= @startDate AND t.CreatedAt < @endDate
                GROUP BY t.ItemId, t.WarehouseId
            ),
            AllPairs AS (
                SELECT DISTINCT t.ItemId, t.WarehouseId
                FROM Transactions t
                UNION
                SELECT sb.ItemId, sb.WarehouseId
                FROM StockBalances sb
            )
            SELECT ap.ItemId, i.Name AS ItemName, i.Article,
                   ap.WarehouseId, w.Name AS WarehouseName,
                   COALESCE(ob.OpenQty, 0) AS OpenBalance,
                   COALESCE(pm.IncomeQty, 0) AS IncomeQty,
                   COALESCE(pm.OutcomeQty, 0) AS OutcomeQty,
                   COALESCE(ob.OpenQty, 0) + COALESCE(pm.IncomeQty, 0) - COALESCE(pm.OutcomeQty, 0) AS CloseBalance
            FROM AllPairs ap
            JOIN Items i ON i.Id = ap.ItemId
            JOIN Warehouses w ON w.Id = ap.WarehouseId
            LEFT JOIN OpenBalance ob ON ob.ItemId = ap.ItemId AND ob.WarehouseId = ap.WarehouseId
            LEFT JOIN PeriodMovements pm ON pm.ItemId = ap.ItemId AND pm.WarehouseId = ap.WarehouseId
            WHERE (@itemId IS NULL OR ap.ItemId = @itemId)
            ORDER BY i.Name, w.Name",
            new { startDate, endDate, itemId });

        return rows.Select(r => new TurnoverReportDto(
            (int)r.ItemId,
            (string)r.ItemName,
            (string?)r.Article,
            (int)r.WarehouseId,
            (string)r.WarehouseName,
            (decimal)r.OpenBalance,
            (decimal)r.IncomeQty,
            (decimal)r.OutcomeQty,
            (decimal)r.CloseBalance
        )).ToList();
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        using var connection = _dapper.CreateConnection();
        using var multi = await connection.QueryMultipleAsync(@"
            SELECT COUNT(*) FROM Items;
            SELECT COALESCE(SUM(Quantity * Price), 0) FROM StockBalances;
            SELECT COUNT(*) FROM Documents
                WHERE Status = 'Posted'
                  AND Date >= DATEADD(MONTH, DATEDIFF(MONTH, 0, GETUTCDATE()), 0);
            SELECT COUNT(*) FROM (
                SELECT sb.ItemId
                FROM StockBalances sb
                JOIN Items i ON i.Id = sb.ItemId
                GROUP BY sb.ItemId, i.MinStock
                HAVING SUM(sb.Quantity) < i.MinStock
            ) AS LowStock;
            SELECT COUNT(*) FROM Warehouses WHERE IsActive = 1;");

        var totalItems = await multi.ReadFirstAsync<int>();
        var totalStockValue = await multi.ReadFirstAsync<decimal>();
        var documentsThisMonth = await multi.ReadFirstAsync<int>();
        var lowStockItems = await multi.ReadFirstOrDefaultAsync<int>();
        var activeWarehouses = await multi.ReadFirstAsync<int>();

        return new DashboardStatsDto(
            totalItems, totalStockValue, documentsThisMonth, lowStockItems, activeWarehouses
        );
    }

}
