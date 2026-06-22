using StockWise.App.Models;
using StockWise.Data;

namespace StockWise.App.Repositories;

public class WarehouseRepository : BaseRepository<Warehouse>
{
    public WarehouseRepository(StockDb context) : base(context) { }
}
