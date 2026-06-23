using StockWise.App.Models;
using StockWise.App.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockWise.App.Services;

public class CategoryService : ICategoryService
{
    private readonly CategoryRepository _repo;

    public CategoryService(CategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<CategoryTreeDto>> GetTreeAsync()
    {
        var nodes = await _repo.GetTreeAsync();
        return nodes.Select(n => new CategoryTreeDto(
            n.Id, n.Name, n.ParentId, n.SortOrder, n.Level, n.Path, n.HasChildren));
    }

    public async Task<IEnumerable<CategoryDto>> GetRootsAsync()
    {
        var roots = await _repo.GetRootsAsync();
        return await ToDtoListAsync(roots);
    }

    public async Task<IEnumerable<CategoryDto>> GetChildrenAsync(int parentId)
    {
        var children = await _repo.GetChildrenAsync(parentId);
        return await ToDtoListAsync(children);
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _repo.GetByIdAsync(id);
        return category is null ? null : await ToDtoAsync(category);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Название категории не может быть пустым");

        var category = new Category
        {
            Name = dto.Name.Trim(),
            ParentId = dto.ParentId,
            SortOrder = dto.SortOrder
        };

        var created = await _repo.AddAsync(category);
        return await ToDtoAsync(created);
    }

    public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Название категории не может быть пустым");

        if (dto.ParentId == id)
            throw new InvalidOperationException("Категория не может быть родителем самой себя");

        var category = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Категория с Id={id} не найдена");

        if (dto.ParentId.HasValue && await IsDescendantAsync(id, dto.ParentId.Value))
            throw new InvalidOperationException("Нельзя переместить категорию в своего потомка");

        category.Name = dto.Name.Trim();
        category.ParentId = dto.ParentId;
        category.SortOrder = dto.SortOrder;

        await _repo.UpdateAsync(category);
        return await ToDtoAsync(category);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (await _repo.HasChildrenAsync(id))
            return false;

        if (await _repo.HasItemsAsync(id))
            return false;

        var category = await _repo.GetByIdAsync(id);
        if (category is null)
            return false;

        await _repo.DeleteAsync(category);
        return true;
    }

    public async Task ReorderAsync(int id, int newSortOrder)
    {
        await _repo.ReorderAsync(id, newSortOrder);
    }

    public async Task<IEnumerable<CategoryDto>> GetPathAsync(int id)
    {
        var path = await _repo.GetPathAsync(id);
        return await ToDtoListAsync(path);
    }

    private async Task<bool> IsDescendantAsync(int ancestorId, int descendantId)
    {
        var all = await _repo.GetAllFlatAsync();
        var visited = new HashSet<int>();
        var stack = new Stack<int>();
        stack.Push(descendantId);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (current == ancestorId)
                return true;

            if (!visited.Add(current))
                continue;

            foreach (var c in all.Where(c => c.ParentId == current))
                stack.Push(c.Id);
        }

        return false;
    }

    private async Task<CategoryDto> ToDtoAsync(Category c)
    {
        var hasChildren = await _repo.HasChildrenAsync(c.Id);
        var hasItems = await _repo.HasItemsAsync(c.Id);
        return new CategoryDto(c.Id, c.Name, c.ParentId, c.SortOrder, hasChildren, hasItems);
    }

    private async Task<List<CategoryDto>> ToDtoListAsync(IEnumerable<Category> categories)
    {
        var result = new List<CategoryDto>();
        foreach (var c in categories)
            result.Add(await ToDtoAsync(c));
        return result;
    }
}
