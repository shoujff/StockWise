using Avalonia.Controls;
using Avalonia.Interactivity;
using StockWise.App.ViewModels;

namespace StockWise.App.Views;

public partial class ItemEditView : UserControl
{
    public ItemEditView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is ItemEditViewModel vm)
        {
            vm.LoadCommand.Execute(null);
        }
    }
}
