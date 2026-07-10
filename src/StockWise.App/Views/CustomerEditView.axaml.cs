using Avalonia.Controls;
using Avalonia.Interactivity;
using StockWise.App.ViewModels;

namespace StockWise.App.Views;

public partial class CustomerEditView : UserControl
{
    public CustomerEditView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is CustomerEditViewModel vm)
        {
            vm.LoadCommand.Execute(null);
        }
    }
}
