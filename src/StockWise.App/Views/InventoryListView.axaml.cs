using Avalonia.Controls;
using Avalonia.Interactivity;
using StockWise.App.ViewModels;

namespace StockWise.App.Views;

public partial class InventoryListView : UserControl
{
    public InventoryListView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is InventoryListViewModel vm)
        {
            vm.LoadCommand.Execute(null);
        }
    }
}
