using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockWise.App.Services;

namespace StockWise.App.ViewModels;

public partial class DocumentListViewModel : ObservableObject
{
    private readonly IDocumentService _documentService;

    [ObservableProperty]
    private ObservableCollection<DocumentListDto> _documents = [];

    [ObservableProperty]
    private string _selectedTypeFilter = "";

    [ObservableProperty]
    private string _selectedStatusFilter = "";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasItems;

    [ObservableProperty]
    private bool _hasNoItems = true;

    public ObservableCollection<string> TypeFilters { get; } = ["", "Income", "Outcome", "Transfer"];
    public ObservableCollection<string> StatusFilters { get; } = ["", "Draft", "Posted", "Cancelled"];

    public event Action<string>? CreateRequested;
    public event Action<int>? EditRequested;
    public event Action<int>? ViewRequested;

    public DocumentListViewModel(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            await SearchAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        IsLoading = true;
        try
        {
            var typeFilter = string.IsNullOrWhiteSpace(SelectedTypeFilter) ? null : SelectedTypeFilter;
            var statusFilter = string.IsNullOrWhiteSpace(SelectedStatusFilter) ? null : SelectedStatusFilter;

            var results = await _documentService.GetAllAsync(typeFilter, statusFilter);
            Documents = [.. results];
            HasItems = Documents.Count > 0;
            HasNoItems = Documents.Count == 0;
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSelectedTypeFilterChanged(string value)
    {
        _ = SearchAsync();
    }

    partial void OnSelectedStatusFilterChanged(string value)
    {
        _ = SearchAsync();
    }

    [RelayCommand]
    private void Create(string type)
    {
        CreateRequested?.Invoke(type);
    }

    [RelayCommand]
    private void Edit(int id)
    {
        EditRequested?.Invoke(id);
    }

    [RelayCommand]
    private void View(int id)
    {
        ViewRequested?.Invoke(id);
    }

    private static string GetTypeDisplayName(string type) => type switch
    {
        "Income" => "Приход",
        "Outcome" => "Расход",
        "Transfer" => "Перемещение",
        _ => type
    };

    private static string GetStatusColor(string status) => status switch
    {
        "Draft" => "Amber",
        "Posted" => "Green",
        "Cancelled" => "Red",
        _ => "Gray"
    };
}
