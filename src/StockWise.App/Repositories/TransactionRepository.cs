using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using StockWise.App.Models;
using StockWise.Data;

namespace StockWise.App.Repositories;

public class TransactionRepository : BaseRepository<Transaction>
{
    private readonly DapperContext _dapper;

    public TransactionRepository(StockDb context, DapperContext dapper) : base(context)
    {
        _dapper = dapper;
    }

    public async Task<IEnumerable<Transaction>> GetByItemAsync(int itemId)
    {
        using var connection = _dapper.CreateConnection();
        return await connection.QueryAsync<Transaction>(
            "SELECT * FROM Transactions WHERE ItemId = @itemId ORDER BY CreatedAt DESC", new { itemId });
    }

    public async Task<IEnumerable<Transaction>> GetByWarehouseAsync(int warehouseId)
    {
        using var connection = _dapper.CreateConnection();
        return await connection.QueryAsync<Transaction>(
            "SELECT * FROM Transactions WHERE WarehouseId = @warehouseId ORDER BY CreatedAt DESC", new { warehouseId });
    }
}
