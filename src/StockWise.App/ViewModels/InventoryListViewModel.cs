using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockWise.App.Services;

namespace StockWise.App.ViewModels;

public partial class InventoryListViewModel : ObservableObject
{
    private readonly IInventoryService _inventoryService;
    private readonly IAuthService _authService;
    private readonly IWarehouseService _warehouseService;

    [ObservableProperty]
    private ObservableCollection<InventoryListDto> _inventories = [];

    [ObservableProperty]
    private ObservableCollection<WarehouseOptionDto> _warehouses = [];

    [ObservableProperty]
    private WarehouseOptionDto? _selectedWarehouse;

    [ObservableProperty]
    private string _selectedStatusFilter = "";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasItems;

    [ObservableProperty]
    private bool _hasNoItems = true;

    [ObservableProperty]
    private string _errorMessage = "";

    [ObservableProperty]
    private bool _hasError;

    public ObservableCollection<string> StatusFilters { get; } = ["", "Draft", "Confirmed", "Cancelled"];

    public event Action<int>? CreateRequested;
    public event Action<int>? EditRequested;
    public event Action<string>? PermissionDenied;

    public InventoryListViewModel(
        IInventoryService inventoryService,
        IAuthService authService,
        IWarehouseService warehouseService)
    {
        _inventoryService = inventoryService;
        _authService = authService;
        _warehouseService = warehouseService;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var whs = await _warehouseService.GetAllAsync();
            Warehouses = [.. whs.Select(w => new WarehouseOptionDto(w.Id, w.Name))];

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
            var statusFilter = string.IsNullOrWhiteSpace(SelectedStatusFilter) ? null : SelectedStatusFilter;
            var results = await _inventoryService.GetAllAsync(statusFilter);
            Inventories = [.. results];
            HasItems = Inventories.Count > 0;
            HasNoItems = Inventories.Count == 0;
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSelectedStatusFilterChanged(string value)
    {
        _ = SearchAsync();
    }

    [RelayCommand]
    private async Task Create()
    {
        if (!await _authService.HasPermissionAsync(_authService.CurrentUser!, Permissions.InventoryCreate))
        {
            PermissionDenied?.Invoke("Нет прав на создание инвентаризации");
            return;
        }

        if (SelectedWarehouse is null)
        {
            HasError = true;
            ErrorMessage = "Выберите склад для инвентаризации";
            return;
        }

        CreateRequested?.Invoke(SelectedWarehouse.Id);
    }

    [RelayCommand]
    private void Edit(int id)
    {
        EditRequested?.Invoke(id);
    }

    private static string GetStatusColor(string status) => status switch
    {
        "Draft" => "Amber",
        "Confirmed" => "Green",
        "Cancelled" => "Red",
        _ => "Gray"
    };
}
