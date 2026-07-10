using Avalonia.Controls;
using Avalonia.Interactivity;
using StockWise.App.ViewModels;

namespace StockWise.App.Views;

public partial class UserEditView : UserControl
{
    public UserEditView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is UserEditViewModel vm)
        {
            vm.LoadCommand.Execute(null);
        }
    }
}
