using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockWise.App.Services;

namespace StockWise.App.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IReportService _reportService;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = "";

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private int _totalItems;

    [ObservableProperty]
    private decimal _totalStockValue;

    [ObservableProperty]
    private string _totalStockValueDisplay = "0";

    [ObservableProperty]
    private int _documentsThisMonth;

    [ObservableProperty]
    private int _lowStockItems;

    [ObservableProperty]
    private int _activeWarehouses;

    [ObservableProperty]
    private AbcAnalysisItemDto? _topItem;

    public DashboardViewModel(IReportService reportService)
    {
        _reportService = reportService;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        HasError = false;
        try
        {
            var stats = await _reportService.GetDashboardStatsAsync();
            TotalItems = stats.TotalItems;
            TotalStockValue = stats.TotalStockValue;
            TotalStockValueDisplay = stats.TotalStockValue.ToString("N2");
            DocumentsThisMonth = stats.DocumentsThisMonth;
            LowStockItems = stats.LowStockItems;
            ActiveWarehouses = stats.ActiveWarehouses;

            var abc = await _reportService.GetAbcAnalysisAsync(
                DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);
            TopItem = abc.Count > 0 ? abc[0] : null;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
