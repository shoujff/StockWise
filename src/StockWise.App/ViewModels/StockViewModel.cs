using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockWise.App.Services;

namespace StockWise.App.ViewModels;

public partial class StockViewModel : ObservableObject
{
    private readonly IStockService _stockService;

    [ObservableProperty]
    private ObservableCollection<WarehouseOptionDto> _warehouses = [];

    [ObservableProperty]
    private WarehouseOptionDto? _selectedWarehouse;

    [ObservableProperty]
    private int? _selectedWarehouseId;

    [ObservableProperty]
    private ObservableCollection<StockItemDto> _stockItems = [];

    [ObservableProperty]
    private string _searchTerm = "";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasItems;

    [ObservableProperty]
    private bool _hasNoItems = true;

    [ObservableProperty]
    private int _totalItems;

    [ObservableProperty]
    private decimal _totalQuantity;

    [ObservableProperty]
    private string _totalQuantityDisplay = "0";

    [ObservableProperty]
    private int _lowStockCount;

    public StockViewModel(IStockService stockService)
    {
        _stockService = stockService;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var whs = await _stockService.GetWarehouseOptionsAsync();
            Warehouses = [.. whs];

            if (SelectedWarehouse is null && Warehouses.Count > 0)
            {
                SelectedWarehouse = Warehouses[0];
            }

            if (SelectedWarehouse is not null)
            {
                await LoadStockAsync();
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSelectedWarehouseChanged(WarehouseOptionDto? value)
    {
        if (value is not null)
        {
            SelectedWarehouseId = value.Id;
            _ = LoadStockAsync();
        }
    }

    private async Task LoadStockAsync()
    {
        if (SelectedWarehouseId is null) return;

        IsLoading = true;
        try
        {
            var items = await _stockService.GetStockByWarehouseAsync(SelectedWarehouseId.Value);

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim().ToLower();
                items = items.Where(i =>
                    i.ItemName.ToLower().Contains(term) ||
                    (i.Article?.ToLower().Contains(term) ?? false) ||
                    (i.Barcode?.ToLower().Contains(term) ?? false));
            }

            var list = items.ToList();
            StockItems = [.. list];
            HasItems = StockItems.Count > 0;
            HasNoItems = StockItems.Count == 0;
            TotalItems = list.Count;
            TotalQuantity = list.Sum(i => i.Quantity);
            TotalQuantityDisplay = TotalQuantity.ToString("N0");
            LowStockCount = list.Count(i => i.IsLowStock);
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task LoadForWarehouseAsync(int warehouseId)
    {
        var whs = await _stockService.GetWarehouseOptionsAsync();
        Warehouses = [.. whs];
        SelectedWarehouse = Warehouses.FirstOrDefault(w => w.Id == warehouseId);
        if (SelectedWarehouse is not null)
        {
            await LoadStockAsync();
        }
    }

    partial void OnSearchTermChanged(string value)
    {
        _ = LoadStockAsync();
    }
}
