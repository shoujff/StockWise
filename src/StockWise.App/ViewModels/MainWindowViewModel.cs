using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    public MainWindowViewModel(IItemService itemService, ICategoryService categoryService)
    {
        _itemService = itemService;
        _categoryService = categoryService;
    }

    public void Initialize()
    {
        NavigateTo(PageType.Dashboard);
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
        return vm;
    }

    private ItemEditViewModel CreateItemEditViewModel()
    {
        var vm = new ItemEditViewModel(_itemService, _categoryService);
        vm.Saved += () => NavigateTo(PageType.Items);
        vm.Cancelled += () => NavigateTo(PageType.Items);
        return vm;
    }

    private CategoryListViewModel CreateCategoryListViewModel()
    {
        return new CategoryListViewModel(_categoryService);
    }
}
