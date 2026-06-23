using Dapper;
using StockWise.App.Models;
using StockWise.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockWise.App.Repositories;

public class CategoryRepository : BaseRepository<Category>
{
    private readonly DapperContext _dapper;

    public CategoryRepository(StockDb context, DapperContext dapper) : base(context)
    {
        _dapper = dapper;
    }

    public async Task<IEnumerable<Category>> GetAllFlatAsync()
    {
        using var conn = _dapper.CreateConnection();
        return await conn.QueryAsync<Category>("SELECT * FROM Categories ORDER BY SortOrder");
    }

    public async Task<IEnumerable<Category>> GetRootsAsync()
    {
        using var conn = _dapper.CreateConnection();
        return await conn.QueryAsync<Category>(
            "SELECT * FROM Categories WHERE ParentId IS NULL ORDER BY SortOrder");
    }

    public async Task<IEnumerable<Category>> GetChildrenAsync(int parentId)
    {
        using var conn = _dapper.CreateConnection();
        return await conn.QueryAsync<Category>(
            "SELECT * FROM Categories WHERE ParentId = @parentId ORDER BY SortOrder",
            new { parentId });
    }

    public async Task<IEnumerable<CategoryTreeNode>> GetTreeAsync()
    {
        using var conn = _dapper.CreateConnection();
        return await conn.QueryAsync<CategoryTreeNode>(@"
            WITH CategoryCTE AS (
                SELECT c.Id, c.Name, c.ParentId, c.SortOrder,
                       0 AS Level,
                       CAST(c.Name AS NVARCHAR(MAX)) AS Path,
                       CAST(CASE WHEN EXISTS (
                           SELECT 1 FROM Categories c2 WHERE c2.ParentId = c.Id
                       ) THEN 1 ELSE 0 END AS BIT) AS HasChildren
                FROM Categories c
                WHERE c.ParentId IS NULL

                UNION ALL

                SELECT c.Id, c.Name, c.ParentId, c.SortOrder,
                       ct.Level + 1,
                       CAST(ct.Path + N' > ' + c.Name AS NVARCHAR(MAX)),
                       CAST(CASE WHEN EXISTS (
                           SELECT 1 FROM Categories c2 WHERE c2.ParentId = c.Id
                       ) THEN 1 ELSE 0 END AS BIT)
                FROM Categories c
                INNER JOIN CategoryCTE ct ON c.ParentId = ct.Id
            )
            SELECT * FROM CategoryCTE ORDER BY Path");
    }

    public async Task<IEnumerable<Category>> GetPathAsync(int categoryId)
    {
        using var conn = _dapper.CreateConnection();
        return await conn.QueryAsync<Category>(@"
            WITH PathCTE AS (
                SELECT Id, Name, ParentId, SortOrder
                FROM Categories WHERE Id = @id

                UNION ALL

                SELECT c.Id, c.Name, c.ParentId, c.SortOrder
                FROM Categories c
                INNER JOIN PathCTE p ON c.Id = p.ParentId
            )
            SELECT * FROM PathCTE ORDER BY Id",
            new { id = categoryId });
    }

    public async Task<bool> HasChildrenAsync(int categoryId)
    {
        using var conn = _dapper.CreateConnection();
        var count = await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM Categories WHERE ParentId = @categoryId",
            new { categoryId });
        return count > 0;
    }

    public async Task<bool> HasItemsAsync(int categoryId)
    {
        using var conn = _dapper.CreateConnection();
        var count = await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM Items WHERE CategoryId = @categoryId",
            new { categoryId });
        return count > 0;
    }

    public async Task ReorderAsync(int id, int newSortOrder)
    {
        using var conn = _dapper.CreateConnection();
        await conn.ExecuteAsync(
            "UPDATE Categories SET SortOrder = @newSortOrder WHERE Id = @id",
            new { id, newSortOrder });
    }
}

public record CategoryTreeNode(
    int Id,
    string Name,
    int? ParentId,
    int SortOrder,
    int Level,
    string Path,
    bool HasChildren
);
