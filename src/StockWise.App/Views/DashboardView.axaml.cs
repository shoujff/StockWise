using Avalonia.Controls;
using Avalonia.Interactivity;
using StockWise.App.ViewModels;

namespace StockWise.App.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is DashboardViewModel vm)
        {
            vm.LoadCommand.Execute(null);
        }
    }
}
