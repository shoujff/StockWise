using Avalonia.Controls;
using Avalonia.Interactivity;
using StockWise.App.ViewModels;

namespace StockWise.App.Views;

public partial class WarehouseEditView : UserControl
{
    public WarehouseEditView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is WarehouseEditViewModel vm)
        {
            vm.LoadCommand.Execute(null);
        }
    }
}
