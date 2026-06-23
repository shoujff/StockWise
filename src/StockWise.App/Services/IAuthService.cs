using System.Threading.Tasks;
using StockWise.App.Models;

namespace StockWise.App.Services;

public interface IAuthService
{
    User? CurrentUser { get; }
    Task<User?> ValidateUserAsync(string login, string password);
    Task<User?> RegisterAsync(string login, string password, string firstName, string lastName, string role);
    Task<bool> HasPermissionAsync(User user, string permission);
    void Login(User user);
    void Logout();
}
