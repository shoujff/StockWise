using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using StockWise.App.Models;
using StockWise.Data;

namespace StockWise.App.Repositories;

public class RolePermissionRepository : BaseRepository<RolePermission>
{
    private readonly DapperContext _dapper;

    public RolePermissionRepository(StockDb context, DapperContext dapper) : base(context)
    {
        _dapper = dapper;
    }

    public async Task<IEnumerable<string>> GetPermissionsByRoleAsync(string role)
    {
        using var connection = _dapper.CreateConnection();
        return await connection.QueryAsync<string>(
            "SELECT Permission FROM RolePermissions WHERE Role = @role", new { role });
    }
}
