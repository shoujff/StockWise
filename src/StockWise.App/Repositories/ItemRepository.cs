using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using StockWise.App.Models;
using StockWise.Data;

namespace StockWise.App.Repositories;

public class ItemRepository : BaseRepository<Item>
{
    private readonly DapperContext _dapper;

    public ItemRepository(StockDb context, DapperContext dapper) : base(context)
    {
        _dapper = dapper;
    }

    public async Task<IEnumerable<Item>> SearchAsync(string? searchTerm, int? categoryId)
    {
        using var connection = _dapper.CreateConnection();
        var sql = "SELECT * FROM Items WHERE 1=1";
        if (!string.IsNullOrWhiteSpace(searchTerm))
            sql += " AND (Name LIKE @search OR Article LIKE @search OR Barcode LIKE @search)";
        if (categoryId.HasValue)
            sql += " AND CategoryId = @categoryId";
        return await connection.QueryAsync<Item>(sql, new { search = $"%{searchTerm}%", categoryId });
    }
}
