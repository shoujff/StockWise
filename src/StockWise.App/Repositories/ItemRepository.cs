using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockWise.App.Models;
using StockWise.Data;

namespace StockWise.App.Repositories;

public class ItemRepository : BaseRepository<Item>
{
    public ItemRepository(StockDb context) : base(context) { }

    public override async Task<List<Item>> GetAllAsync()
    {
        return await _dbSet.Include(x => x.Category).ToListAsync();
    }

    public override async Task<Item?> GetByIdAsync<TId>(TId id)
    {
        return await _dbSet.Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id.Equals(id));
    }

    public async Task<IEnumerable<Item>> SearchAsync(string? searchTerm, int? categoryId)
    {
        var query = _dbSet.Include(x => x.Category).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(x =>
                x.Name.Contains(term) ||
                x.Article.Contains(term) ||
                (x.Barcode != null && x.Barcode.Contains(term)));
        }

        if (categoryId.HasValue)
            query = query.Where(x => x.CategoryId == categoryId.Value);

        return await query.ToListAsync();
    }

    public async Task<Item?> GetByArticleAsync(string article)
    {
        return await _dbSet.Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Article == article);
    }

    public async Task<Item?> GetByBarcodeAsync(string barcode)
    {
        return await _dbSet.Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Barcode == barcode);
    }

    public async Task<string?> GetLastArticleAsync()
    {
        return await _dbSet.OrderByDescending(x => x.Id)
            .Select(x => x.Article)
            .FirstOrDefaultAsync();
    }
}
