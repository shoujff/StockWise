using Avalonia.Controls;
using StockWise.App.ViewModels;

namespace StockWise.App;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}