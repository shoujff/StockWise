using Avalonia.Controls;
using Avalonia.Interactivity;
using StockWise.App.ViewModels;

namespace StockWise.App.Views;

public partial class OrderListView : UserControl
{
    public OrderListView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is OrderListViewModel vm)
        {
            vm.LoadCommand.Execute(null);
        }
    }
}
