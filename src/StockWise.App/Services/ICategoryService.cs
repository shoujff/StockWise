using StockWise.App.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockWise.App.Services;

public record CategoryDto(
    int Id,
    string Name,
    int? ParentId,
    int SortOrder,
    bool HasChildren,
    bool HasItems
);

public record CategoryTreeDto(
    int Id,
    string Name,
    int? ParentId,
    int SortOrder,
    int Level,
    string Path,
    bool HasChildren
);

public record CreateCategoryDto(
    string Name,
    int? ParentId,
    int SortOrder = 0
);

public record UpdateCategoryDto(
    string Name,
    int? ParentId,
    int SortOrder
);

public interface ICategoryService
{
    Task<IEnumerable<CategoryTreeDto>> GetTreeAsync();
    Task<IEnumerable<CategoryDto>> GetRootsAsync();
    Task<IEnumerable<CategoryDto>> GetChildrenAsync(int parentId);
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
    Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto dto);
    Task<bool> DeleteAsync(int id);
    Task ReorderAsync(int id, int newSortOrder);
    Task<IEnumerable<CategoryDto>> GetPathAsync(int id);
}
