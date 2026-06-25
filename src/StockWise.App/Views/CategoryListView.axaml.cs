using Avalonia.Controls;
using Avalonia.Interactivity;
using StockWise.App.ViewModels;

namespace StockWise.App.Views;

public partial class CategoryListView : UserControl
{
    public CategoryListView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is CategoryListViewModel vm)
        {
            vm.LoadCommand.Execute(null);
        }
    }
}
