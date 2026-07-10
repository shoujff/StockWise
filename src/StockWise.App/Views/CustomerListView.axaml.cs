using Avalonia.Controls;
using Avalonia.Interactivity;
using StockWise.App.ViewModels;

namespace StockWise.App.Views;

public partial class CustomerListView : UserControl
{
    public CustomerListView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is CustomerListViewModel vm)
        {
            vm.LoadCommand.Execute(null);
        }
    }
}
