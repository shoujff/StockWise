using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using StockWise.App.Models;
using StockWise.App.Repositories;

namespace StockWise.App.Services;

public class AuthService : IAuthService
{
    private readonly UserRepository _userRepo;
    private readonly RolePermissionRepository _rolePermissionRepo;
    private readonly ConcurrentDictionary<string, HashSet<string>> _permissionCache = new();

    public User? CurrentUser { get; private set; }

    public AuthService(UserRepository userRepo, RolePermissionRepository rolePermissionRepo)
    {
        _userRepo = userRepo;
        _rolePermissionRepo = rolePermissionRepo;
    }

    public async Task<User?> ValidateUserAsync(string login, string password)
    {
        var user = await _userRepo.GetByLoginAsync(login);
        if (user == null || !VerifyPassword(password, user.PasswordHash))
            return null;
        return user;
    }

    public async Task<User?> RegisterAsync(string login, string password, string firstName, string lastName, string role)
    {
        var exists = await _userRepo.GetByLoginAsync(login);
        if (exists != null)
            return null;

        var user = new User
        {
            Login = login,
            PasswordHash = HashPassword(password),
            FirstName = firstName,
            LastName = lastName,
            Role = role
        };
        return await _userRepo.AddAsync(user);
    }

    public async Task<bool> HasPermissionAsync(User user, string permission)
    {
        if (user.Role == "Admin")
            return true;

        if (!_permissionCache.TryGetValue(user.Role, out var permissions))
        {
            var perms = await _rolePermissionRepo.GetPermissionsByRoleAsync(user.Role);
            permissions = new HashSet<string>(perms);
            _permissionCache[user.Role] = permissions;
        }

        return permissions.Contains(permission);
    }

    public void Login(User user)
    {
        CurrentUser = user;
    }

    public void Logout()
    {
        CurrentUser = null;
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
