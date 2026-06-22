using StockWise.App.Models;
using StockWise.Data;

namespace StockWise.App.Repositories;

public class CategoryRepository : BaseRepository<Category>
{
    public CategoryRepository(StockDb context) : base(context) { }
}
