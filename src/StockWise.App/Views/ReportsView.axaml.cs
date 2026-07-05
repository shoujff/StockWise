using Avalonia.Controls;
using Avalonia.Interactivity;
using StockWise.App.ViewModels;

namespace StockWise.App.Views;

public partial class ReportsView : UserControl
{
    public ReportsView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is ReportsViewModel vm)
        {
            vm.GenerateCommand.Execute(null);
        }
    }
}
