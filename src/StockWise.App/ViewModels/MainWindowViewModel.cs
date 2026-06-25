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
    Documents,
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
        ToastService toastService)
    {
        _itemService = itemService;
        _categoryService = categoryService;
        _authService = authService;
        _themeService = themeService;
        _toastService = toastService;
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
        ActivePage = page;
        ActivePageName = page switch
        {
            PageType.Dashboard => "Дашборд",
            PageType.Items => "Номенклатура",
            PageType.ItemEdit => "Редактирование товара",
            PageType.Categories => "Категории",
            PageType.Warehouses => "Склады",
            PageType.Documents => "Документы",
            PageType.Orders => "Заказы",
            PageType.Inventory => "Инвентаризация",
            PageType.Reports => "Отчёты",
            PageType.Users => "Пользователи",
            PageType.Settings => "Настройки",
            _ => ""
        };

        CurrentPageViewModel = page switch
        {
            PageType.Items => CreateItemListViewModel(),
            PageType.ItemEdit => CreateItemEditViewModel(),
            PageType.Categories => CreateCategoryListViewModel(),
            _ => null
        };
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
}
