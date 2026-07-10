using System.Threading.Tasks;
using Dapper;
using StockWise.App.Models;
using StockWise.Data;

namespace StockWise.App.Repositories;

public class CustomerRepository : BaseRepository<Customer>
{
    private readonly DapperContext _dapper;

    public CustomerRepository(StockDb context, DapperContext dapper) : base(context)
    {
        _dapper = dapper;
    }

    public async Task<Customer?> GetByNameAsync(string name)
    {
        using var connection = _dapper.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Customer>(
            "SELECT * FROM Customers WHERE Name = @name", new { name });
    }
}
