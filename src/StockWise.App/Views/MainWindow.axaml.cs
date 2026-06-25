using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using StockWise.App.Services;
using StockWise.App.ViewModels;

namespace StockWise.App.Views;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;

    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;
        _viewModel.LogoutRequested += OnLogoutRequested;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        _viewModel.Initialize();
    }

    private void OnLogoutRequested()
    {
        var authService = App.Services!.GetRequiredService<IAuthService>();
        var loginWindow = new LoginWindow(authService);
        loginWindow.Show();

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = loginWindow;
        }

        Close();
    }
}
