using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockWise.App.Services;

public record CustomerDto(
    int Id,
    string Name,
    string? INN,
    string? ContactPerson,
    string? Phone,
    string? Email
);

public record CreateCustomerDto(
    string Name,
    string? INN,
    string? ContactPerson,
    string? Phone,
    string? Email
);

public record UpdateCustomerDto(
    string Name,
    string? INN,
    string? ContactPerson,
    string? Phone,
    string? Email
);

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllAsync();
    Task<CustomerDto?> GetByIdAsync(int id);
    Task<CustomerDto> CreateAsync(CreateCustomerDto dto);
    Task<CustomerDto> UpdateAsync(int id, UpdateCustomerDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<CustomerDto>> SearchAsync(string? searchTerm);
}
