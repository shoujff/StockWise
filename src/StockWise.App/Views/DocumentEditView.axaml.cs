using Avalonia.Controls;
using Avalonia.Interactivity;
using StockWise.App.ViewModels;

namespace StockWise.App.Views;

public partial class DocumentEditView : UserControl
{
    public DocumentEditView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is DocumentEditViewModel vm)
        {
            vm.LoadCommand.Execute(null);
        }
    }
}
