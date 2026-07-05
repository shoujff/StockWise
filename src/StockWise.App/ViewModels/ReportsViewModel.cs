using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockWise.App.Services;

namespace StockWise.App.ViewModels;

public partial class ReportsViewModel : ObservableObject
{
    private readonly IReportService _reportService;

    [ObservableProperty]
    private DateTimeOffset _startDate = DateTimeOffset.Now.AddMonths(-1);

    [ObservableProperty]
    private DateTimeOffset _endDate = DateTimeOffset.Now;

    [ObservableProperty]
    private int _selectedReportIndex;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasAbcData;

    [ObservableProperty]
    private bool _hasTurnoverData;

    [ObservableProperty]
    private bool _isAbcVisible = true;

    [ObservableProperty]
    private bool _isTurnoverVisible;

    [ObservableProperty]
    private ObservableCollection<AbcAnalysisItemDto> _abcItems = [];

    [ObservableProperty]
    private ObservableCollection<TurnoverReportDto> _turnoverItems = [];

    public string[] ReportTypes { get; } = ["ABC-анализ", "Оборотно-сальдовая"];

    public ReportsViewModel(IReportService reportService)
    {
        _reportService = reportService;
    }

    partial void OnSelectedReportIndexChanged(int value)
    {
        IsAbcVisible = value == 0;
        IsTurnoverVisible = value == 1;
    }

    [ObservableProperty]
    private string _errorMessage = "";

    [ObservableProperty]
    private bool _hasError;

    [RelayCommand]
    private async Task GenerateAsync()
    {
        IsLoading = true;
        HasError = false;
        try
        {
            var start = StartDate.DateTime;
            var end = EndDate.DateTime;

            if (SelectedReportIndex == 0)
            {
                var items = await _reportService.GetAbcAnalysisAsync(start, end);
                AbcItems = [.. items];
                HasAbcData = AbcItems.Count > 0;
            }
            else
            {
                var items = await _reportService.GetTurnoverReportAsync(start, end);
                TurnoverItems = [.. items];
                HasTurnoverData = TurnoverItems.Count > 0;
            }
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
