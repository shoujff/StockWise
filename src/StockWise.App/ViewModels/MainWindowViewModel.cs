using CommunityToolkit.Mvvm.ComponentModel;

namespace StockWise.App.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string _version = "v1.0.0";

    public MainWindowViewModel()
    {
    }
}