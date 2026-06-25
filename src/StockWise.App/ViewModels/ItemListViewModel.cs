using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockWise.App.Services;

namespace StockWise.App.ViewModels;

public partial class ItemListViewModel : ObservableObject
{
    private readonly IItemService _itemService;
    private readonly ICategoryService _categoryService;

    [ObservableProperty]
    private ObservableCollection<ItemDto> _items = [];

    [ObservableProperty]
    private ObservableCollection<CategoryTreeDto> _categories = [];

    [ObservableProperty]
    private string _searchTerm = "";

    [ObservableProperty]
    private int? _selectedCategoryId;

    [ObservableProperty]
    private CategoryTreeDto? _selectedCategory;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasItems;

    [ObservableProperty]
    private bool _hasNoItems = true;

    public event Action? CreateRequested;
    public event Action<int>? EditRequested;
    public event Action? ItemDeleted;

    public ItemListViewModel(IItemService itemService, ICategoryService categoryService)
    {
        _itemService = itemService;
        _categoryService = categoryService;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var cats = await _categoryService.GetTreeAsync();
            Categories = [new CategoryTreeDto(0, "Все категории", null, -1, -1, "Все категории", false), .. cats];

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
            var results = await _itemService.SearchAsync(
                string.IsNullOrWhiteSpace(SearchTerm) ? null : SearchTerm,
                SelectedCategoryId);
            Items = [.. results];
            HasItems = Items.Count > 0;
            HasNoItems = Items.Count == 0;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DeleteAsync(int id)
    {
        var success = await _itemService.DeleteAsync(id);
        if (success)
        {
            Items.Remove(Items.FirstOrDefault(i => i.Id == id));
            HasItems = Items.Count > 0;
            HasNoItems = Items.Count == 0;
            ItemDeleted?.Invoke();
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

    partial void OnSearchTermChanged(string value)
    {
        _ = SearchAsync();
    }

    partial void OnSelectedCategoryIdChanged(int? value)
    {
        _ = SearchAsync();
    }

    partial void OnSelectedCategoryChanged(CategoryTreeDto? value)
    {
        SelectedCategoryId = value is null || value.Id == 0 ? null : value.Id;
    }
}
