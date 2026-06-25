using Avalonia.Controls;
using Avalonia.Interactivity;
using StockWise.App.ViewModels;

namespace StockWise.App.Views;

public partial class ItemListView : UserControl
{
    public ItemListView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is ItemListViewModel vm)
        {
            vm.LoadCommand.Execute(null);
        }
    }
}
