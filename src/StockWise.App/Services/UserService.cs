using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockWise.App.Models;
using StockWise.App.Repositories;

namespace StockWise.App.Services;

public class UserService : IUserService
{
    private readonly UserRepository _userRepo;
    private readonly IAuthService _authService;

    public UserService(UserRepository userRepo, IAuthService authService)
    {
        _userRepo = userRepo;
        _authService = authService;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepo.GetAllAsync();
        return users.Select(ToDto);
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _userRepo.GetByIdAsync(id);
        return user is null ? null : ToDto(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Login))
            throw new ArgumentException("Логин не может быть пустым");
        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new ArgumentException("Пароль не может быть пустым");
        if (string.IsNullOrWhiteSpace(dto.FirstName))
            throw new ArgumentException("Имя не может быть пустым");

        var existing = await _userRepo.GetByLoginAsync(dto.Login);
        if (existing != null)
            throw new InvalidOperationException("Пользователь с таким логином уже существует");

        var user = new User
        {
            Login = dto.Login.Trim(),
            PasswordHash = _authService.HashPassword(dto.Password),
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName?.Trim() ?? "",
            Role = dto.Role
        };

        var created = await _userRepo.AddAsync(user);
        return ToDto(created);
    }

    public async Task<UserDto> UpdateAsync(int id, UpdateUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Login))
            throw new ArgumentException("Логин не может быть пустым");
        if (string.IsNullOrWhiteSpace(dto.FirstName))
            throw new ArgumentException("Имя не может быть пустым");

        var user = await _userRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Пользователь с Id={id} не найден");

        var existing = await _userRepo.GetByLoginAsync(dto.Login);
        if (existing != null && existing.Id != id)
            throw new InvalidOperationException("Пользователь с таким логином уже существует");

        user.Login = dto.Login.Trim();
        user.FirstName = dto.FirstName.Trim();
        user.LastName = dto.LastName?.Trim() ?? "";
        user.Role = dto.Role;

        await _userRepo.UpdateAsync(user);
        return ToDto(user);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user is null)
            return false;

        if (user.Login == "admin")
            throw new InvalidOperationException("Невозможно удалить учётную запись администратора");

        if (_authService.CurrentUser?.Id == id)
            throw new InvalidOperationException("Невозможно удалить собственную учётную запись");

        await _userRepo.DeleteAsync(user);
        return true;
    }

    public async Task<bool> ResetPasswordAsync(int id, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword))
            throw new ArgumentException("Пароль не может быть пустым");

        var user = await _userRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Пользователь с Id={id} не найден");

        user.PasswordHash = _authService.HashPassword(newPassword);
        await _userRepo.UpdateAsync(user);
        return true;
    }

    public async Task<IEnumerable<UserDto>> SearchAsync(string? searchTerm)
    {
        var all = await _userRepo.GetAllAsync();
        if (string.IsNullOrWhiteSpace(searchTerm))
            return all.Select(ToDto);

        var term = searchTerm.Trim().ToLower();
        return all.Where(u =>
            u.Login.ToLower().Contains(term) ||
            u.FirstName.ToLower().Contains(term) ||
            u.LastName.ToLower().Contains(term) ||
            u.Role.ToLower().Contains(term))
            .Select(ToDto);
    }

    private static UserDto ToDto(User u) => new(
        u.Id, u.FirstName, u.LastName, u.Login, u.Role);
}
