using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockWise.App.Services;

public record WarehouseDto(
    int Id,
    string Name,
    string? Address,
    bool IsActive,
    int StockCount
);

public record CreateWarehouseDto(
    string Name,
    string? Address
);

public record UpdateWarehouseDto(
    string Name,
    string? Address,
    bool IsActive
);

public interface IWarehouseService
{
    Task<IEnumerable<WarehouseDto>> GetAllAsync();
    Task<WarehouseDto?> GetByIdAsync(int id);
    Task<WarehouseDto> CreateAsync(CreateWarehouseDto dto);
    Task<WarehouseDto> UpdateAsync(int id, UpdateWarehouseDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<WarehouseDto>> SearchAsync(string? searchTerm);
}
