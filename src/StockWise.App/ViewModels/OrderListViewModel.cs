using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockWise.App.Services;

namespace StockWise.App.ViewModels;

public partial class OrderListViewModel : ObservableObject
{
    private readonly IOrderService _orderService;
    private readonly IAuthService _authService;

    [ObservableProperty]
    private ObservableCollection<OrderListDto> _orders = [];

    [ObservableProperty]
    private string _selectedStatusFilter = "";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasItems;

    [ObservableProperty]
    private bool _hasNoItems = true;

    public ObservableCollection<string> StatusFilters { get; } = ["", "New", "InProgress", "Shipped", "Cancelled"];

    public event Action? CreateRequested;
    public event Action<int>? EditRequested;
    public event Action<int>? ShipRequested;
    public event Action<string>? PermissionDenied;

    public OrderListViewModel(IOrderService orderService, IAuthService authService)
    {
        _orderService = orderService;
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
            var statusFilter = string.IsNullOrWhiteSpace(SelectedStatusFilter) ? null : SelectedStatusFilter;

            var filter = new OrderFilterDto(statusFilter, null, null, null);
            var results = await _orderService.GetAllAsync(filter);
            Orders = [.. results];
            HasItems = Orders.Count > 0;
            HasNoItems = Orders.Count == 0;
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
        if (!await _authService.HasPermissionAsync(_authService.CurrentUser!, Permissions.OrdersCreate))
        {
            PermissionDenied?.Invoke("Нет прав на создание заказов");
            return;
        }
        CreateRequested?.Invoke();
    }

    [RelayCommand]
    private void Edit(int id)
    {
        EditRequested?.Invoke(id);
    }

    [RelayCommand]
    private void Ship(int id)
    {
        ShipRequested?.Invoke(id);
    }

    private static string GetStatusColor(string status) => status switch
    {
        "New" => "Amber",
        "InProgress" => "Blue",
        "Shipped" => "Green",
        "Cancelled" => "Red",
        _ => "Gray"
    };
}
