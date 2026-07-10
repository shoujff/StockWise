using Avalonia.Controls;
using Avalonia.Interactivity;
using StockWise.App.ViewModels;

namespace StockWise.App.Views;

public partial class UserListView : UserControl
{
    public UserListView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is UserListViewModel vm)
        {
            vm.LoadCommand.Execute(null);
        }
    }
}
