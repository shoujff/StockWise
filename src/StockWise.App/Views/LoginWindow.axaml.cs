using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using StockWise.App.Models;
using StockWise.App.Services;
using StockWise.App.ViewModels;

namespace StockWise.App.Views;

public partial class LoginWindow : Window
{
    private readonly IAuthService _authService;

    public LoginWindow(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;

        ShowLoginView();
    }

    private void OnLoginSucceeded(User user)
    {
        var mainWindow = App.Services!.GetRequiredService<MainWindow>();
        mainWindow.Show();

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = mainWindow;
        }

        Dispatcher.UIThread.Post(() => Close());
    }

    private void OnRegisterSucceeded(User user)
    {
        var mainWindow = App.Services!.GetRequiredService<MainWindow>();
        mainWindow.Show();

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = mainWindow;
        }

        Dispatcher.UIThread.Post(() => Close());
    }

    private void ShowLoginView()
    {
        var vm = new LoginViewModel(_authService);
        vm.LoginSucceeded += OnLoginSucceeded;
        vm.GoToRegisterRequested += ShowRegisterView;
        ContentArea.Content = vm;
    }

    private void ShowRegisterView()
    {
        var vm = new RegisterViewModel(_authService);
        vm.RegisterSucceeded += OnRegisterSucceeded;
        vm.GoToLoginRequested += ShowLoginView;
        ContentArea.Content = vm;
    }
}
