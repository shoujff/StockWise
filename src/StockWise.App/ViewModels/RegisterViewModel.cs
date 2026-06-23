using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockWise.App.Models;
using StockWise.App.Services;

namespace StockWise.App.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string _username = "";

    [ObservableProperty]
    private string _email = "";

    [ObservableProperty]
    private string _firstName = "";

    [ObservableProperty]
    private string _lastName = "";

    [ObservableProperty]
    private string _password = "";

    [ObservableProperty]
    private string _confirmPassword = "";

    [ObservableProperty]
    private string _errorMessage = "";

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private bool _isLoading;

    public event Action<User>? RegisterSucceeded;
    public event Action? GoToLoginRequested;

    public RegisterViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        HasError = false;
        IsLoading = true;

        if (string.IsNullOrWhiteSpace(Username) ||
            string.IsNullOrWhiteSpace(Password) ||
            string.IsNullOrWhiteSpace(FirstName) ||
            string.IsNullOrWhiteSpace(LastName))
        {
            ErrorMessage = "Заполните все обязательные поля";
            HasError = true;
            IsLoading = false;
            return;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Пароли не совпадают";
            HasError = true;
            IsLoading = false;
            return;
        }

        if (Password.Length < 6)
        {
            ErrorMessage = "Пароль должен содержать минимум 6 символов";
            HasError = true;
            IsLoading = false;
            return;
        }

        var user = await _authService.RegisterAsync(Username, Password, FirstName, LastName, "Viewer");
        if (user is null)
        {
            ErrorMessage = "Пользователь с таким именем уже существует";
            HasError = true;
            IsLoading = false;
            return;
        }

        _authService.Login(user);
        RegisterSucceeded?.Invoke(user);
    }

    [RelayCommand]
    private void GoToLogin()
    {
        GoToLoginRequested?.Invoke();
    }
}
