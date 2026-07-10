using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockWise.App.Services;

namespace StockWise.App.ViewModels;

public partial class UserListViewModel : ObservableObject
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    [ObservableProperty]
    private ObservableCollection<UserDto> _users = [];

    [ObservableProperty]
    private string _searchTerm = "";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasItems;

    [ObservableProperty]
    private bool _hasNoItems = true;

    public event Action? CreateRequested;
    public event Action<int>? EditRequested;
    public event Action? UserDeleted;
    public event Action<string>? PermissionDenied;

    public UserListViewModel(IUserService userService, IAuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            await SearchAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        IsLoading = true;
        try
        {
            var results = await _userService.SearchAsync(
                string.IsNullOrWhiteSpace(SearchTerm) ? null : SearchTerm);
            Users = [.. results];
            HasItems = Users.Count > 0;
            HasNoItems = Users.Count == 0;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DeleteAsync(int id)
    {
        if (!await _authService.HasPermissionAsync(_authService.CurrentUser!, Permissions.AdminUsersDelete))
        {
            PermissionDenied?.Invoke("Нет прав на удаление пользователей");
            return;
        }

        var success = await _userService.DeleteAsync(id);
        if (success)
        {
            var item = Users.FirstOrDefault(u => u.Id == id);
            if (item is not null) Users.Remove(item);
            HasItems = Users.Count > 0;
            HasNoItems = Users.Count == 0;
            UserDeleted?.Invoke();
        }
    }

    [RelayCommand]
    private void Create()
    {
        CreateRequested?.Invoke();
    }

    [RelayCommand]
    private void Edit(int id)
    {
        EditRequested?.Invoke(id);
    }

    partial void OnSearchTermChanged(string value)
    {
        _ = SearchAsync();
    }
}
