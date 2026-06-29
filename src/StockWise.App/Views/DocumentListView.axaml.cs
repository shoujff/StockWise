using Avalonia.Controls;
using Avalonia.Interactivity;
using StockWise.App.ViewModels;

namespace StockWise.App.Views;

public partial class DocumentListView : UserControl
{
    public DocumentListView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is DocumentListViewModel vm)
        {
            vm.LoadCommand.Execute(null);
        }
    }
}
