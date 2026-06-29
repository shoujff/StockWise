using Avalonia.Controls;
using Avalonia.Interactivity;
using StockWise.App.ViewModels;

namespace StockWise.App.Views;

public partial class WarehouseListView : UserControl
{
    public WarehouseListView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is WarehouseListViewModel vm)
        {
            vm.LoadCommand.Execute(null);
        }
    }
}
