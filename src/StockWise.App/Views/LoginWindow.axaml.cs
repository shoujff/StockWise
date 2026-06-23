using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using StockWise.App.Models;
using StockWise.App.Services;
using StockWise.App.ViewModels;

namespace StockWise.App.Views;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _loginVM;
    private readonly RegisterViewModel _registerVM;

    public LoginWindow(IAuthService authService)
    {
        InitializeComponent();

        _loginVM = new LoginViewModel(authService);
        _registerVM = new RegisterViewModel(authService);

        _loginVM.LoginSucceeded += OnAuthSucceeded;
        _registerVM.RegisterSucceeded += OnAuthSucceeded;
        _loginVM.GoToRegisterRequested += () => ShowView(_registerVM);
        _registerVM.GoToLoginRequested += () => ShowView(_loginVM);

        ShowView(_loginVM);
    }

    private void ShowView(object vm)
    {
        ContentArea.Content = vm;
    }

    private void OnAuthSucceeded(User user)
    {
        var mainWindow = App.Services!.GetRequiredService<MainWindow>();
        mainWindow.Show();

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = mainWindow;
        }

        Close();
    }
}
