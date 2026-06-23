using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
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

    public async Task<Item?> GetByArticleAsync(string article)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Article == article);
    }

    public async Task<Item?> GetByBarcodeAsync(string barcode)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Barcode == barcode);
    }

    public async Task<string?> GetLastArticleAsync()
    {
        return await _dbSet.OrderByDescending(x => x.Id)
            .Select(x => x.Article)
            .FirstOrDefaultAsync();
    }
}
