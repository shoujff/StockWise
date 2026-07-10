using System.Collections.Generic;
using System.Threading.Tasks;
using StockWise.App.Models;

namespace StockWise.App.Services;

public record UserDto(
    int Id,
    string FirstName,
    string LastName,
    string Login,
    string Role
);

public record CreateUserDto(
    string FirstName,
    string LastName,
    string Login,
    string Password,
    string Role
);

public record UpdateUserDto(
    string FirstName,
    string LastName,
    string Login,
    string Role
);

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(int id);
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task<UserDto> UpdateAsync(int id, UpdateUserDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ResetPasswordAsync(int id, string newPassword);
    Task<IEnumerable<UserDto>> SearchAsync(string? searchTerm);
}
