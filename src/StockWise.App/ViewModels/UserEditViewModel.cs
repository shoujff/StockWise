using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockWise.App.Services;

namespace StockWise.App.ViewModels;

public partial class UserEditViewModel : ObservableObject
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private int? _editingId;

    [ObservableProperty]
    private string _pageTitle = "Новый пользователь";

    [ObservableProperty]
    private string _firstName = "";

    [ObservableProperty]
    private string _lastName = "";

    [ObservableProperty]
    private string _login = "";

    [ObservableProperty]
    private string _password = "";

    [ObservableProperty]
    private string _selectedRole = "Viewer";

    [ObservableProperty]
    private string _errorMessage = "";

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isSaving;

    [ObservableProperty]
    private bool _isNotSaving = true;

    [ObservableProperty]
    private bool _isPasswordEnabled = true;

    public string[] Roles { get; } = ["Admin", "Manager", "Warehouse", "Viewer"];

    public event Action? Saved;
    public event Action? Cancelled;
    public event Action<string>? PermissionDenied;

    public UserEditViewModel(IUserService userService, IAuthService authService)
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
            if (_editingId.HasValue)
            {
                var user = await _userService.GetByIdAsync(_editingId.Value);
                if (user is not null)
                {
                    FirstName = user.FirstName;
                    LastName = user.LastName;
                    Login = user.Login;
                    SelectedRole = user.Role;
                    IsPasswordEnabled = false;
                }
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task LoadForEditAsync(int id)
    {
        _editingId = id;
        PageTitle = "Редактирование пользователя";
        await LoadAsync();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var perm = _editingId.HasValue ? Permissions.AdminUsersEdit : Permissions.AdminUsersCreate;
        if (!await _authService.HasPermissionAsync(_authService.CurrentUser!, perm))
        {
            PermissionDenied?.Invoke("Нет прав на сохранение пользователя");
            return;
        }

        HasError = false;
        IsSaving = true;
        IsNotSaving = false;

        try
        {
            if (_editingId.HasValue)
            {
                await _userService.UpdateAsync(_editingId.Value, new UpdateUserDto(
                    FirstName.Trim(), LastName.Trim(), Login.Trim(), SelectedRole));
            }
            else
            {
                await _userService.CreateAsync(new CreateUserDto(
                    FirstName.Trim(), LastName.Trim(), Login.Trim(), Password, SelectedRole));
            }

            Saved?.Invoke();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
        finally
        {
            IsSaving = false;
            IsNotSaving = true;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        Cancelled?.Invoke();
    }
}
