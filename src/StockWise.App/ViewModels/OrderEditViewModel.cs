using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockWise.App.Services;

namespace StockWise.App.ViewModels;

public partial class OrderLineEditDto : ObservableObject
{
    [ObservableProperty] private int _itemId;
    [ObservableProperty] private string _itemName = "";
    [ObservableProperty] private string _article = "";
    [ObservableProperty] private string _unit = "";
    [ObservableProperty] private decimal _quantity = 1;
    [ObservableProperty] private decimal _price;
    [ObservableProperty] private decimal _amount;
    [ObservableProperty] private decimal _shippedQty;

    partial void OnQuantityChanged(decimal value) => Amount = value * Price;
    partial void OnPriceChanged(decimal value) => Amount = value * Quantity;
}

public partial class CustomerOptionDto : ObservableObject
{
    [ObservableProperty] private int _id;
    [ObservableProperty] private string _name = "";

    public CustomerOptionDto(int id, string name)
    {
        Id = id;
        Name = name;
    }
}

public partial class OrderEditViewModel : ObservableObject
{
    private readonly IOrderService _orderService;
    private readonly IItemService _itemService;
    private readonly IWarehouseService _warehouseService;
    private readonly ICustomerService _customerService;
    private readonly IAuthService _authService;
    private int? _editingId;

    [ObservableProperty]
    private string _pageTitle = "Новый заказ";

    [ObservableProperty]
    private string _orderNumber = "";

    [NotifyPropertyChangedFor(nameof(IsNew))]
    [NotifyPropertyChangedFor(nameof(IsInProgress))]
    [NotifyPropertyChangedFor(nameof(IsShipped))]
    [NotifyPropertyChangedFor(nameof(IsCancelled))]
    [NotifyPropertyChangedFor(nameof(CanEdit))]
    [ObservableProperty]
    private string _status = "";

    [ObservableProperty]
    private bool _hasOrder;

    [ObservableProperty]
    private string? _customerName;

    [ObservableProperty]
    private ObservableCollection<WarehouseOptionDto> _warehouses = [];

    [ObservableProperty]
    private WarehouseOptionDto? _selectedWarehouse;

    [ObservableProperty]
    private ObservableCollection<CustomerOptionDto> _customers = [];

    [ObservableProperty]
    private CustomerOptionDto? _selectedCustomer;

    public ObservableCollection<OrderLineEditDto> Lines { get; } = [];

    [ObservableProperty]
    private string _searchItemTerm = "";

    [ObservableProperty]
    private ObservableCollection<ItemDto> _searchResults = [];

    [ObservableProperty]
    private bool _hasSearchResults;

    [ObservableProperty]
    private ItemDto? _selectedSearchItem;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isSaving;

    [ObservableProperty]
    private string _errorMessage = "";

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private decimal _totalAmount;

    [ObservableProperty]
    private ObservableCollection<OrderLineDetailDto> _originalLines = [];

    public bool IsNew => Status == "New";
    public bool IsInProgress => Status == "InProgress";
    public bool IsShipped => Status == "Shipped";
    public bool IsCancelled => Status == "Cancelled";
    public bool CanEdit => Status is "" or "New";

    public event Action? Saved;
    public event Action? Cancelled;
    public event Action<string>? PermissionDenied;

    public OrderEditViewModel(
        IOrderService orderService,
        IItemService itemService,
        IWarehouseService warehouseService,
        ICustomerService customerService,
        IAuthService authService)
    {
        _orderService = orderService;
        _itemService = itemService;
        _warehouseService = warehouseService;
        _customerService = customerService;
        _authService = authService;
    }

    private int CurrentUserId => _authService.CurrentUser?.Id ?? 0;

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var whs = await _warehouseService.GetAllAsync();
            Warehouses = [.. whs.Select(w => new WarehouseOptionDto(w.Id, w.Name))];

            var custs = await _customerService.GetAllAsync();
            Customers = [.. custs.Select(c => new CustomerOptionDto(c.Id, c.Name))];

            if (_editingId is null)
            {
                OrderNumber = await _orderService.GenerateNumberAsync();
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task LoadForEditAsync(int id)
    {
        _editingId = id;
        HasOrder = true;
        var order = await _orderService.GetByIdAsync(id);
        if (order is null) return;

        OrderNumber = order.Number;
        Status = order.Status;
        CustomerName = order.CustomerName;
        TotalAmount = order.TotalAmount;
        PageTitle = $"Заказ #{order.Number}";

        await LoadAsync();

        if (order.CustomerName is not null)
        {
            SelectedCustomer = Customers.FirstOrDefault(c => c.Name == order.CustomerName);
        }

        Lines.Clear();
        foreach (var l in order.Lines)
        {
            Lines.Add(new OrderLineEditDto
            {
                ItemId = l.ItemId,
                ItemName = l.ItemName ?? "",
                Article = l.Article ?? "",
                Unit = l.Unit ?? "",
                Quantity = l.Quantity,
                Price = l.Price,
                Amount = l.Amount,
                ShippedQty = l.ShippedQty
            });
        }

        OriginalLines = [.. order.Lines];

        RecalculateTotal();
    }

    [RelayCommand]
    private async Task SearchItemAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchItemTerm))
        {
            SearchResults = [];
            HasSearchResults = false;
            return;
        }

        var results = await _itemService.SearchAsync(SearchItemTerm, null);
        SearchResults = [.. results];
        HasSearchResults = SearchResults.Count > 0;
    }

    partial void OnSearchItemTermChanged(string value)
    {
        _ = SearchItemAsync();
    }

    partial void OnSelectedCustomerChanged(CustomerOptionDto? value)
    {
        CustomerName = value?.Name;
    }

    partial void OnSelectedSearchItemChanged(ItemDto? value)
    {
        if (value is null) return;
        AddLineFromItem(value);
    }

    [RelayCommand]
    private void AddLine()
    {
        if (SelectedSearchItem is null) return;
        AddLineFromItem(SelectedSearchItem);
    }

    private void AddLineFromItem(ItemDto item)
    {
        if (Lines.Any(l => l.ItemId == item.Id))
            return;

        Lines.Add(new OrderLineEditDto
        {
            ItemId = item.Id,
            ItemName = item.Name,
            Article = item.Article,
            Unit = item.Unit,
            Quantity = 1,
            Price = item.Price,
            Amount = item.Price
        });

        SelectedSearchItem = null;
        SearchItemTerm = "";
        SearchResults = [];
        HasSearchResults = false;
        RecalculateTotal();
    }

    [RelayCommand]
    private void RemoveLine(OrderLineEditDto line)
    {
        if (!CanEdit) return;
        Lines.Remove(line);
        RecalculateTotal();
    }

    private void RecalculateTotal()
    {
        TotalAmount = Lines.Sum(l => l.Amount);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (!await _authService.HasPermissionAsync(_authService.CurrentUser!, Permissions.OrdersCreate))
        {
            PermissionDenied?.Invoke("Нет прав на создание заказов");
            return;
        }

        if (!CanEdit) return;

        HasError = false;
        IsSaving = true;

        try
        {
            if (Lines.Count == 0)
                throw new ArgumentException("Добавьте хотя бы одну строку");

            var createLines = Lines.Select(l => new CreateOrderLineDto(
                l.ItemId, l.Quantity, l.Price)).ToList();

            var dto = new CreateOrderDto(SelectedCustomer?.Id, CustomerName, createLines);

            await _orderService.CreateAsync(dto, CurrentUserId);

            Saved?.Invoke();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private async Task ConfirmAsync()
    {
        if (_editingId is null) return;

        if (!await _authService.HasPermissionAsync(_authService.CurrentUser!, Permissions.OrdersEdit))
        {
            PermissionDenied?.Invoke("Нет прав на подтверждение заказов");
            return;
        }

        HasError = false;
        IsSaving = true;

        try
        {
            await _orderService.ConfirmAsync(_editingId.Value, CurrentUserId);
            Status = "InProgress";
            Saved?.Invoke();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private async Task ShipAsync()
    {
        if (_editingId is null) return;

        if (!await _authService.HasPermissionAsync(_authService.CurrentUser!, Permissions.OrdersEdit))
        {
            PermissionDenied?.Invoke("Нет прав на отгрузку заказов");
            return;
        }

        HasError = false;
        IsSaving = true;

        try
        {
            await _orderService.ShipAsync(_editingId.Value, CurrentUserId);

            var updated = await _orderService.GetByIdAsync(_editingId.Value);
            if (updated is not null)
            {
                Status = updated.Status;
                Lines.Clear();
                foreach (var l in updated.Lines)
                {
                    Lines.Add(new OrderLineEditDto
                    {
                        ItemId = l.ItemId,
                        ItemName = l.ItemName ?? "",
                        Article = l.Article ?? "",
                        Unit = l.Unit ?? "",
                        Quantity = l.Quantity,
                        Price = l.Price,
                        Amount = l.Amount,
                        ShippedQty = l.ShippedQty
                    });
                }
            }

            Saved?.Invoke();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private async Task CancelOrderAsync()
    {
        if (_editingId is null) return;

        if (!await _authService.HasPermissionAsync(_authService.CurrentUser!, Permissions.OrdersEdit))
        {
            PermissionDenied?.Invoke("Нет прав на отмену заказов");
            return;
        }

        HasError = false;
        IsSaving = true;

        try
        {
            await _orderService.CancelAsync(_editingId.Value);
            Status = "Cancelled";
            Saved?.Invoke();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        Cancelled?.Invoke();
    }
}
