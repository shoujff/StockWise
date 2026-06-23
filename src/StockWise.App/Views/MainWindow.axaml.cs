using Avalonia.Controls;
using StockWise.App.ViewModels;

namespace StockWise.App.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}