using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockWise.App.Models;
using StockWise.App.Services;

namespace StockWise.App.ViewModels;

public enum PageType
{
    Dashboard,
    Items,
    ItemEdit,
    Categories,
    Warehouses,
    WarehouseEdit,
    Stock,
    Documents,
    DocumentEdit,
    Orders,
    OrderEdit,
    Inventory,
    Reports,
    Users,
    UserEdit,
    Customers,
    CustomerEdit
}

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IItemService _itemService;
    private readonly ICategoryService _categoryService;
    private readonly IAuthService _authService;
    private readonly ThemeService _themeService;
    private readonly ToastService _toastService;
    private readonly IWarehouseService _warehouseService;
    private readonly IStockService _stockService;
    private readonly IDocumentService _documentService;
    private readonly IReportService _reportService;
    private readonly IOrderService _orderService;
    private readonly IInventoryService _inventoryService;
    private readonly IUserService _userService;
    private readonly ICustomerService _customerService;

    [ObservableProperty]
    private string _version = "v1.0.0";

    [ObservableProperty]
    private string _activePageName = "Дашборд";

    [ObservableProperty]
    private string _currentDate = DateTime.Now.ToString("dd MMMM yyyy");

    [ObservableProperty]
    private PageType _activePage = PageType.Dashboard;

    [ObservableProperty]
    private object? _currentPageViewModel;

    [ObservableProperty]
    private string _themeIcon = "🌙";

    [ObservableProperty]
    private DialogViewModel _dialog = new();

    [ObservableProperty]
    private string _userDisplayName = "";

    [ObservableProperty]
    private string _userEmail = "";

    [ObservableProperty]
    private string _userRole = "";

    [ObservableProperty]
    private bool _canViewItems = true;

    [ObservableProperty]
    private bool _canViewCategories;

    [ObservableProperty]
    private bool _canViewWarehouses = true;

    [ObservableProperty]
    private bool _canViewStock = true;

    [ObservableProperty]
    private bool _canViewDocuments = true;

    [ObservableProperty]
    private bool _canViewOrders;

    [ObservableProperty]
    private bool _canViewInventory;

    [ObservableProperty]
    private bool _canViewReports = true;

    [ObservableProperty]
    private bool _canViewUsers;

    [ObservableProperty]
    private bool _canViewCustomers;

    public ObservableCollection<ToastNotification> Toasts => _toastService.Toasts;

    public event Action? LogoutRequested;

    public MainWindowViewModel(
        IItemService itemService,
        ICategoryService categoryService,
        IAuthService authService,
        ThemeService themeService,
        ToastService toastService,
        IWarehouseService warehouseService,
        IStockService stockService,
        IDocumentService documentService,
        IReportService reportService,
        IOrderService orderService,
        IInventoryService inventoryService,
        IUserService userService,
        ICustomerService customerService)
    {
        _itemService = itemService;
        _inventoryService = inventoryService;
        _userService = userService;
        _customerService = customerService;
        _categoryService = categoryService;
        _authService = authService;
        _themeService = themeService;
        _toastService = toastService;
        _warehouseService = warehouseService;
        _stockService = stockService;
        _documentService = documentService;
        _reportService = reportService;
        _orderService = orderService;
        _themeService.ThemeChanged += OnThemeChanged;
    }

    private void OnThemeChanged(AppTheme theme)
    {
        ThemeIcon = theme == AppTheme.Dark ? "☀️" : "🌙";
    }

    public void Initialize()
    {
        UpdateUserInfo();
        NavigateTo(PageType.Dashboard);
    }

    private void UpdateUserInfo()
    {
        var user = _authService.CurrentUser;
        if (user is null) return;

        UserDisplayName = user.GetFullName();
        UserEmail = user.Login;
        UserRole = user.Role;

        var role = user.Role;
        CanViewItems = true;
        CanViewCategories = role is "Admin" or "Manager";
        CanViewWarehouses = true;
        CanViewStock = true;
        CanViewDocuments = true;
        CanViewOrders = role is "Admin" or "Manager";
        CanViewInventory = role is "Admin" or "Warehouse";
        CanViewReports = true;
        CanViewUsers = role is "Admin";
        CanViewCustomers = role is "Admin" or "Manager";
    }

    public void Cleanup()
    {
        CurrentPageViewModel = null;
        ActivePage = PageType.Dashboard;
    }

    public void ShowToast(ToastType type, string message, int durationMs = 3000)
    {
        _toastService.Show(type, message, durationMs);
    }

    public void Confirm(string title, string message,
        Action<bool>? callback, string confirmText = "OK",
        string cancelText = "Отмена")
    {
        Dialog.Show(title, message, confirmText, cancelText, true, callback);
    }

    private bool IsPageAllowed(PageType page)
    {
        var role = _authService.CurrentUser?.Role ?? "";
        if (role == "Admin") return true;

        return page switch
        {
            PageType.Items => true,
            PageType.ItemEdit => role == "Manager",
            PageType.Categories => role == "Manager",
            PageType.Warehouses => true,
            PageType.WarehouseEdit => role == "Manager",
            PageType.Stock => true,
            PageType.Documents => true,
            PageType.DocumentEdit => role == "Manager",
            PageType.Orders => role == "Manager",
            PageType.OrderEdit => role == "Manager",
            PageType.Inventory => role == "Warehouse",
            PageType.Reports => true,
            PageType.Users => false,
            PageType.UserEdit => false,
            PageType.Customers => role is "Admin" or "Manager",
            PageType.CustomerEdit => role is "Admin" or "Manager",
            PageType.Dashboard => true,
            _ => false
        };
    }

    [RelayCommand]
    private void NavigateTo(PageType page)
    {
        try
        {
            if (!IsPageAllowed(page))
            {
                ShowToast(ToastType.Warning, "Недостаточно прав для просмотра этой страницы");
                return;
            }

            ActivePage = page;
            ActivePageName = page switch
            {
                PageType.Dashboard => "Дашборд",
                PageType.Items => "Номенклатура",
                PageType.ItemEdit => "Редактирование товара",
                PageType.Categories => "Категории",
                PageType.Warehouses => "Склады",
                PageType.WarehouseEdit => "Редактирование склада",
                PageType.Stock => "Остатки",
                PageType.Documents => "Документы",
                PageType.DocumentEdit => "Редактирование документа",
                PageType.Orders => "Заказы",
                PageType.OrderEdit => "Редактирование заказа",
                PageType.Inventory => "Инвентаризация",
                PageType.Reports => "Отчёты",
                PageType.Users => "Пользователи",
                PageType.UserEdit => "Редактирование пользователя",
                PageType.Customers => "Контрагенты",
                PageType.CustomerEdit => "Редактирование контрагента",
                _ => ""
            };

            CurrentPageViewModel = page switch
            {
                PageType.Dashboard => CreateDashboardViewModel(),
                PageType.Items => CreateItemListViewModel(),
                PageType.ItemEdit => CreateItemEditViewModel(),
                PageType.Categories => CreateCategoryListViewModel(),
                PageType.Warehouses => CreateWarehouseListViewModel(),
                PageType.WarehouseEdit => CreateWarehouseEditViewModel(),
                PageType.Stock => CreateStockViewModel(),
                PageType.Documents => CreateDocumentListViewModel(),
                PageType.DocumentEdit => CreateDocumentEditViewModel(),
                PageType.Orders => CreateOrderListViewModel(),
                PageType.OrderEdit => CreateOrderEditViewModel(),
                PageType.Inventory => CreateInventoryListViewModel(),
                PageType.Reports => CreateReportsViewModel(),
                PageType.Users => CreateUserListViewModel(),
                PageType.UserEdit => CreateUserEditViewModel(),
                PageType.Customers => CreateCustomerListViewModel(),
                PageType.CustomerEdit => CreateCustomerEditViewModel(),
                _ => null
            };
        }
        catch (Exception ex)
        {
            System.IO.File.AppendAllText("crash.log",
                $"[{DateTime.Now}] NavigateTo error: {ex}{Environment.NewLine}");
        }
    }

    [RelayCommand]
    private void NavigateToDocumentEdit(string type)
    {
        if (!IsPageAllowed(PageType.DocumentEdit))
        {
            ShowToast(ToastType.Warning, "Недостаточно прав");
            return;
        }
        ActivePage = PageType.DocumentEdit;
        ActivePageName = "Новый документ";
        var vm = CreateDocumentEditViewModel();
        vm.Initialize(type);
        CurrentPageViewModel = vm;
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        _themeService.Toggle();
    }

    [RelayCommand]
    private void Logout()
    {
        _authService.Logout();
        LogoutRequested?.Invoke();
    }

    [RelayCommand]
    private void NavigateToItemEdit(int id)
    {
        if (!IsPageAllowed(PageType.ItemEdit))
        {
            ShowToast(ToastType.Warning, "Недостаточно прав");
            return;
        }
        ActivePage = PageType.ItemEdit;
        ActivePageName = "Редактирование товара";
        var vm = CreateItemEditViewModel();
        _ = vm.LoadForEditAsync(id);
        CurrentPageViewModel = vm;
    }

    private ItemListViewModel CreateItemListViewModel()
    {
        var vm = new ItemListViewModel(_itemService, _categoryService, _authService);
        vm.EditRequested += (id) => NavigateToItemEdit(id);
        vm.CreateRequested += () => NavigateTo(PageType.ItemEdit);
        vm.ItemDeleted += () => _toastService.Success("Товар удалён");
        vm.ItemDeleteError += (msg) => _toastService.Error(msg);
        vm.PermissionDenied += (msg) => _toastService.Warning(msg);
        return vm;
    }

    private ItemEditViewModel CreateItemEditViewModel()
    {
        var vm = new ItemEditViewModel(_itemService, _categoryService, _authService);
        vm.Saved += () =>
        {
            _toastService.Success("Товар сохранён");
            NavigateTo(PageType.Items);
        };
        vm.Cancelled += () => NavigateTo(PageType.Items);
        vm.PermissionDenied += (msg) => _toastService.Warning(msg);
        return vm;
    }

    private CategoryListViewModel CreateCategoryListViewModel()
    {
        var vm = new CategoryListViewModel(_categoryService, _authService);
        vm.CategorySaved += () => _toastService.Success("Категория сохранена");
        vm.CategoryDeleted += () => _toastService.Success("Категория удалена");
        vm.PermissionDenied += (msg) => _toastService.Warning(msg);
        return vm;
    }

    [RelayCommand]
    private void NavigateToWarehouseEdit(int id)
    {
        if (!IsPageAllowed(PageType.WarehouseEdit))
        {
            ShowToast(ToastType.Warning, "Недостаточно прав");
            return;
        }
        ActivePage = PageType.WarehouseEdit;
        ActivePageName = "Редактирование склада";
        var vm = CreateWarehouseEditViewModel();
        _ = vm.LoadForEditAsync(id);
        CurrentPageViewModel = vm;
    }

    [RelayCommand]
    private void NavigateToStock(int id)
    {
        ActivePage = PageType.Stock;
        ActivePageName = "Остатки";
        var vm = CreateStockViewModel();
        CurrentPageViewModel = vm;
        _ = vm.LoadForWarehouseAsync(id);
    }

    private WarehouseListViewModel CreateWarehouseListViewModel()
    {
        var vm = new WarehouseListViewModel(_warehouseService, _authService);
        vm.EditRequested += (id) => NavigateToWarehouseEdit(id);
        vm.CreateRequested += () => NavigateTo(PageType.WarehouseEdit);
        vm.ViewStockRequested += (id) => NavigateToStock(id);
        vm.WarehouseDeleted += () => _toastService.Success("Склад удалён");
        vm.PermissionDenied += (msg) => _toastService.Warning(msg);
        return vm;
    }

    private WarehouseEditViewModel CreateWarehouseEditViewModel()
    {
        var vm = new WarehouseEditViewModel(_warehouseService, _authService);
        vm.Saved += () =>
        {
            _toastService.Success("Склад сохранён");
            NavigateTo(PageType.Warehouses);
        };
        vm.Cancelled += () => NavigateTo(PageType.Warehouses);
        vm.PermissionDenied += (msg) => _toastService.Warning(msg);
        return vm;
    }

    private StockViewModel CreateStockViewModel()
    {
        var vm = new StockViewModel(_stockService);
        return vm;
    }

    private DocumentListViewModel CreateDocumentListViewModel()
    {
        var vm = new DocumentListViewModel(_documentService, _authService);
        vm.CreateRequested += (type) => NavigateToDocumentEdit(type);
        vm.EditRequested += (id) => NavigateToDocumentEdit(id);
        vm.ViewRequested += (id) => NavigateToDocumentEdit(id);
        vm.PermissionDenied += (msg) => _toastService.Warning(msg);
        return vm;
    }

    private DocumentEditViewModel CreateDocumentEditViewModel()
    {
        var vm = new DocumentEditViewModel(_documentService, _itemService, _warehouseService, _authService);
        vm.Saved += () =>
        {
            _toastService.Success("Документ сохранён");
            NavigateTo(PageType.Documents);
        };
        vm.Cancelled += () => NavigateTo(PageType.Documents);
        vm.PermissionDenied += (msg) => _toastService.Warning(msg);
        return vm;
    }

    private void NavigateToDocumentEdit(int id)
    {
        ActivePage = PageType.DocumentEdit;
        ActivePageName = "Редактирование документа";
        var vm = CreateDocumentEditViewModel();
        _ = vm.LoadForEditAsync(id);
        CurrentPageViewModel = vm;
    }

    [RelayCommand]
    private void NavigateToOrderEdit(int id)
    {
        if (!IsPageAllowed(PageType.OrderEdit))
        {
            ShowToast(ToastType.Warning, "Недостаточно прав");
            return;
        }
        ActivePage = PageType.OrderEdit;
        ActivePageName = "Редактирование заказа";
        var vm = CreateOrderEditViewModel();
        _ = vm.LoadForEditAsync(id);
        CurrentPageViewModel = vm;
    }

    private OrderListViewModel CreateOrderListViewModel()
    {
        var vm = new OrderListViewModel(_orderService, _authService);
        vm.EditRequested += (id) => NavigateToOrderEdit(id);
        vm.CreateRequested += () =>
        {
            ActivePage = PageType.OrderEdit;
            ActivePageName = "Новый заказ";
            var editVm = CreateOrderEditViewModel();
            editVm.LoadCommand.Execute(null);
            CurrentPageViewModel = editVm;
        };
        vm.PermissionDenied += (msg) => _toastService.Warning(msg);
        return vm;
    }

    private OrderEditViewModel CreateOrderEditViewModel()
    {
        var vm = new OrderEditViewModel(_orderService, _itemService, _warehouseService, _customerService, _authService);
        vm.Saved += () =>
        {
            _toastService.Success("Заказ сохранён");
            NavigateTo(PageType.Orders);
        };
        vm.Cancelled += () => NavigateTo(PageType.Orders);
        vm.PermissionDenied += (msg) => _toastService.Warning(msg);
        return vm;
    }

    private DashboardViewModel CreateDashboardViewModel()
    {
        return new DashboardViewModel(_reportService);
    }

    private InventoryListViewModel CreateInventoryListViewModel()
    {
        var vm = new InventoryListViewModel(_inventoryService, _authService, _warehouseService);
        vm.CreateRequested += (warehouseId) =>
        {
            ActivePage = PageType.Inventory;
            ActivePageName = "Новая инвентаризация";
            var editVm = CreateInventoryEditViewModel();
            _ = editVm.LoadForCreateAsync(warehouseId);
            CurrentPageViewModel = editVm;
        };
        vm.EditRequested += (id) =>
        {
            ActivePage = PageType.Inventory;
            ActivePageName = "Инвентаризация";
            var editVm = CreateInventoryEditViewModel();
            _ = editVm.LoadForViewAsync(id);
            CurrentPageViewModel = editVm;
        };
        vm.PermissionDenied += (msg) => _toastService.Warning(msg);
        return vm;
    }

    private InventoryEditViewModel CreateInventoryEditViewModel()
    {
        var vm = new InventoryEditViewModel(_inventoryService, _authService);
        vm.Saved += () =>
        {
            _toastService.Success("Инвентаризация сохранена");
            NavigateTo(PageType.Inventory);
        };
        vm.Cancelled += () => NavigateTo(PageType.Inventory);
        vm.PermissionDenied += (msg) => _toastService.Warning(msg);
        return vm;
    }

    private ReportsViewModel CreateReportsViewModel()
    {
        var vm = new ReportsViewModel(_reportService, _authService);
        vm.PermissionDenied += (msg) => _toastService.Warning(msg);
        return vm;
    }

    private UserListViewModel CreateUserListViewModel()
    {
        var vm = new UserListViewModel(_userService, _authService);
        vm.EditRequested += (id) => NavigateToUserEdit(id);
        vm.CreateRequested += () => NavigateTo(PageType.UserEdit);
        vm.UserDeleted += () => _toastService.Success("Пользователь удалён");
        vm.PermissionDenied += (msg) => _toastService.Warning(msg);
        return vm;
    }

    private UserEditViewModel CreateUserEditViewModel()
    {
        var vm = new UserEditViewModel(_userService, _authService);
        vm.Saved += () =>
        {
            _toastService.Success("Пользователь сохранён");
            NavigateTo(PageType.Users);
        };
        vm.Cancelled += () => NavigateTo(PageType.Users);
        vm.PermissionDenied += (msg) => _toastService.Warning(msg);
        return vm;
    }

    [RelayCommand]
    private void NavigateToUserEdit(int id)
    {
        if (!IsPageAllowed(PageType.UserEdit))
        {
            ShowToast(ToastType.Warning, "Недостаточно прав");
            return;
        }
        ActivePage = PageType.UserEdit;
        ActivePageName = "Редактирование пользователя";
        var vm = CreateUserEditViewModel();
        _ = vm.LoadForEditAsync(id);
        CurrentPageViewModel = vm;
    }

    private CustomerListViewModel CreateCustomerListViewModel()
    {
        var vm = new CustomerListViewModel(_customerService, _authService);
        vm.EditRequested += (id) => NavigateToCustomerEdit(id);
        vm.CreateRequested += () => NavigateTo(PageType.CustomerEdit);
        vm.CustomerDeleted += () => _toastService.Success("Контрагент удалён");
        vm.PermissionDenied += (msg) => _toastService.Warning(msg);
        return vm;
    }

    private CustomerEditViewModel CreateCustomerEditViewModel()
    {
        var vm = new CustomerEditViewModel(_customerService, _authService);
        vm.Saved += () =>
        {
            _toastService.Success("Контрагент сохранён");
            NavigateTo(PageType.Customers);
        };
        vm.Cancelled += () => NavigateTo(PageType.Customers);
        vm.PermissionDenied += (msg) => _toastService.Warning(msg);
        return vm;
    }

    [RelayCommand]
    private void NavigateToCustomerEdit(int id)
    {
        if (!IsPageAllowed(PageType.CustomerEdit))
        {
            ShowToast(ToastType.Warning, "Недостаточно прав");
            return;
        }
        ActivePage = PageType.CustomerEdit;
        ActivePageName = "Редактирование контрагента";
        var vm = CreateCustomerEditViewModel();
        _ = vm.LoadForEditAsync(id);
        CurrentPageViewModel = vm;
    }
}
