using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using StockWise.App.Models;
using StockWise.Data;

namespace StockWise.App.Repositories;

public class StockBalanceRepository : BaseRepository<StockBalance>
{
    private readonly DapperContext _dapper;

    public StockBalanceRepository(StockDb context, DapperContext dapper) : base(context)
    {
        _dapper = dapper;
    }

    public async Task<IEnumerable<StockBalance>> GetByWarehouseAsync(int warehouseId)
    {
        using var connection = _dapper.CreateConnection();
        return await connection.QueryAsync<StockBalance>(
            "SELECT * FROM StockBalances WHERE WarehouseId = @warehouseId", new { warehouseId });
    }

    public async Task<StockBalance?> GetByItemAndWarehouseAsync(int itemId, int warehouseId)
    {
        using var connection = _dapper.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<StockBalance>(
            "SELECT * FROM StockBalances WHERE ItemId = @itemId AND WarehouseId = @warehouseId",
            new { itemId, warehouseId });
    }
}
