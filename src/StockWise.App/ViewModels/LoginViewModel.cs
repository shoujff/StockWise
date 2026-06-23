using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockWise.App.Models;
using StockWise.App.Services;

namespace StockWise.App.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string _username = "";

    [ObservableProperty]
    private string _password = "";

    [ObservableProperty]
    private string _errorMessage = "";

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private bool _isLoading;

    public event Action<User>? LoginSucceeded;
    public event Action? GoToRegisterRequested;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        HasError = false;
        IsLoading = true;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Заполните все поля";
            HasError = true;
            IsLoading = false;
            return;
        }

        var user = await _authService.ValidateUserAsync(Username, Password);
        if (user is null)
        {
            ErrorMessage = "Неверное имя пользователя или пароль";
            HasError = true;
            IsLoading = false;
            return;
        }

        _authService.Login(user);
        LoginSucceeded?.Invoke(user);
    }

    [RelayCommand]
    private void GoToRegister()
    {
        GoToRegisterRequested?.Invoke();
    }
}
