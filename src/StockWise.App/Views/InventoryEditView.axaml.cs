using Avalonia.Controls;
using Avalonia.Interactivity;
using StockWise.App.ViewModels;

namespace StockWise.App.Views;

public partial class InventoryEditView : UserControl
{
    public InventoryEditView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is InventoryEditViewModel vm && !vm.HasInventory)
        {
            vm.LoadCommand.Execute(null);
        }
    }
}
