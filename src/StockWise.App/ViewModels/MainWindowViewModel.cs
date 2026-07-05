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
    Inventory,
    Reports,
    Users,
    Settings
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
        IReportService reportService)
    {
        _itemService = itemService;
        _categoryService = categoryService;
        _authService = authService;
        _themeService = themeService;
        _toastService = toastService;
        _warehouseService = warehouseService;
        _stockService = stockService;
        _documentService = documentService;
        _reportService = reportService;
        _themeService.ThemeChanged += OnThemeChanged;
    }

    private void OnThemeChanged(AppTheme theme)
    {
        ThemeIcon = theme == AppTheme.Dark ? "☀️" : "🌙";
    }

    public void Initialize()
    {
        NavigateTo(PageType.Dashboard);
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

    [RelayCommand]
    private void NavigateTo(PageType page)
    {
        try
        {
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
                PageType.Inventory => "Инвентаризация",
                PageType.Reports => "Отчёты",
                PageType.Users => "Пользователи",
                PageType.Settings => "Настройки",
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
                PageType.Reports => CreateReportsViewModel(),
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
        ActivePage = PageType.ItemEdit;
        ActivePageName = "Редактирование товара";
        var vm = CreateItemEditViewModel();
        _ = vm.LoadForEditAsync(id);
        CurrentPageViewModel = vm;
    }

    private ItemListViewModel CreateItemListViewModel()
    {
        var vm = new ItemListViewModel(_itemService, _categoryService);
        vm.EditRequested += (id) => NavigateToItemEdit(id);
        vm.CreateRequested += () => NavigateTo(PageType.ItemEdit);
        vm.ItemDeleted += () => _toastService.Success("Товар удалён");
        vm.ItemDeleteError += (msg) => _toastService.Error(msg);
        return vm;
    }

    private ItemEditViewModel CreateItemEditViewModel()
    {
        var vm = new ItemEditViewModel(_itemService, _categoryService);
        vm.Saved += () =>
        {
            _toastService.Success("Товар сохранён");
            NavigateTo(PageType.Items);
        };
        vm.Cancelled += () => NavigateTo(PageType.Items);
        return vm;
    }

    private CategoryListViewModel CreateCategoryListViewModel()
    {
        var vm = new CategoryListViewModel(_categoryService);
        vm.CategorySaved += () => _toastService.Success("Категория сохранена");
        vm.CategoryDeleted += () => _toastService.Success("Категория удалена");
        return vm;
    }

    [RelayCommand]
    private void NavigateToWarehouseEdit(int id)
    {
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
        var vm = new WarehouseListViewModel(_warehouseService);
        vm.EditRequested += (id) => NavigateToWarehouseEdit(id);
        vm.CreateRequested += () => NavigateTo(PageType.WarehouseEdit);
        vm.ViewStockRequested += (id) => NavigateToStock(id);
        vm.WarehouseDeleted += () => _toastService.Success("Склад удалён");
        return vm;
    }

    private WarehouseEditViewModel CreateWarehouseEditViewModel()
    {
        var vm = new WarehouseEditViewModel(_warehouseService);
        vm.Saved += () =>
        {
            _toastService.Success("Склад сохранён");
            NavigateTo(PageType.Warehouses);
        };
        vm.Cancelled += () => NavigateTo(PageType.Warehouses);
        return vm;
    }

    private StockViewModel CreateStockViewModel()
    {
        var vm = new StockViewModel(_stockService);
        return vm;
    }

    private DocumentListViewModel CreateDocumentListViewModel()
    {
        var vm = new DocumentListViewModel(_documentService);
        vm.CreateRequested += (type) => NavigateToDocumentEdit(type);
        vm.EditRequested += (id) => NavigateToDocumentEdit(id);
        vm.ViewRequested += (id) => NavigateToDocumentEdit(id);
        return vm;
    }

    private DocumentEditViewModel CreateDocumentEditViewModel()
    {
        var vm = new DocumentEditViewModel(_documentService, _itemService, _warehouseService);
        vm.Saved += () =>
        {
            _toastService.Success("Документ сохранён");
            NavigateTo(PageType.Documents);
        };
        vm.Cancelled += () => NavigateTo(PageType.Documents);
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

    private DashboardViewModel CreateDashboardViewModel()
    {
        return new DashboardViewModel(_reportService);
    }

    private ReportsViewModel CreateReportsViewModel()
    {
        return new ReportsViewModel(_reportService);
    }
}
