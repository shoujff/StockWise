using System.Threading.Tasks;
using Dapper;
using StockWise.App.Models;
using StockWise.Data;

namespace StockWise.App.Repositories;

public class UserRepository : BaseRepository<User>
{
    private readonly DapperContext _dapper;

    public UserRepository(StockDb context, DapperContext dapper) : base(context)
    {
        _dapper = dapper;
    }

    public async Task<User?> GetByLoginAsync(string login)
    {
        using var connection = _dapper.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Login = @login", new { login });
    }
}
