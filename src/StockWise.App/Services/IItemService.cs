using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockWise.App.Services;

public record ItemDto(
    int Id,
    string Name,
    string Article,
    string Unit,
    decimal Price,
    decimal MinStock,
    decimal MaxStock,
    bool IsBatch,
    string? Barcode,
    string? ImagePath,
    int CategoryId,
    string CategoryName
);

public record CreateItemDto(
    string Name,
    string Article,
    string Unit,
    decimal Price = 0,
    decimal MinStock = 0,
    decimal MaxStock = 0,
    bool IsBatch = false,
    string? Barcode = null,
    int CategoryId = 1
);

public record UpdateItemDto(
    string Name,
    string Article,
    string Unit,
    decimal Price,
    decimal MinStock,
    decimal MaxStock,
    bool IsBatch,
    string? Barcode,
    int CategoryId
);

public interface IItemService
{
    Task<IEnumerable<ItemDto>> GetAllAsync();
    Task<ItemDto?> GetByIdAsync(int id);
    Task<IEnumerable<ItemDto>> SearchAsync(string? searchTerm, int? categoryId);
    Task<ItemDto> CreateAsync(CreateItemDto dto);
    Task<ItemDto> UpdateAsync(int id, UpdateItemDto dto);
    Task<bool> DeleteAsync(int id);
    Task<ItemDto?> GetByArticleAsync(string article);
    Task<ItemDto?> GetByBarcodeAsync(string barcode);
    Task<bool> IsArticleUniqueAsync(string article, int? excludeId = null);
    Task<bool> IsBarcodeUniqueAsync(string barcode, int? excludeId = null);
    Task<string> GetNextArticleAsync();
}
