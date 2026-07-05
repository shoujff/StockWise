using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockWise.App.Services;

namespace StockWise.App.ViewModels;

public partial class WarehouseListViewModel : ObservableObject
{
    private readonly IWarehouseService _warehouseService;
    private readonly IAuthService _authService;

    [ObservableProperty]
    private ObservableCollection<WarehouseDto> _warehouses = [];

    [ObservableProperty]
    private string _searchTerm = "";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasItems;

    [ObservableProperty]
    private bool _hasNoItems = true;

    public event Action? CreateRequested;
    public event Action<int>? EditRequested;
    public event Action<int>? ViewStockRequested;
    public event Action? WarehouseDeleted;
    public event Action<string>? PermissionDenied;

    public WarehouseListViewModel(IWarehouseService warehouseService, IAuthService authService)
    {
        _warehouseService = warehouseService;
        _authService = authService;
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
            var results = await _warehouseService.SearchAsync(
                string.IsNullOrWhiteSpace(SearchTerm) ? null : SearchTerm);
            Warehouses = [.. results];
            HasItems = Warehouses.Count > 0;
            HasNoItems = Warehouses.Count == 0;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DeleteAsync(int id)
    {
        if (!await _authService.HasPermissionAsync(_authService.CurrentUser!, Permissions.WarehouseDelete))
        {
            PermissionDenied?.Invoke("Нет прав на удаление складов");
            return;
        }
        var success = await _warehouseService.DeleteAsync(id);
        if (success)
        {
            Warehouses.Remove(Warehouses.FirstOrDefault(w => w.Id == id));
            HasItems = Warehouses.Count > 0;
            HasNoItems = Warehouses.Count == 0;
            WarehouseDeleted?.Invoke();
        }
    }

    [RelayCommand]
    private void Create()
    {
        CreateRequested?.Invoke();
    }

    [RelayCommand]
    private void Edit(int id)
    {
        EditRequested?.Invoke(id);
    }

    [RelayCommand]
    private void ViewStock(int id)
    {
        ViewStockRequested?.Invoke(id);
    }

    partial void OnSearchTermChanged(string value)
    {
        _ = SearchAsync();
    }
}
