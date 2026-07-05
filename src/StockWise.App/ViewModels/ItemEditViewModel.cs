using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockWise.App.Services;

namespace StockWise.App.ViewModels;

public partial class ItemEditViewModel : ObservableObject
{
    private readonly IItemService _itemService;
    private readonly ICategoryService _categoryService;
    private readonly IAuthService _authService;
    private int? _editingId;

    [ObservableProperty]
    private string _pageTitle = "Новый товар";

    [ObservableProperty]
    private string _name = "";

    [ObservableProperty]
    private string _article = "";

    [ObservableProperty]
    private string _unit = "";

    [ObservableProperty]
    private decimal _price;

    [ObservableProperty]
    private decimal _minStock;

    [ObservableProperty]
    private decimal _maxStock;

    [ObservableProperty]
    private bool _isBatch;

    [ObservableProperty]
    private string? _barcode;

    [ObservableProperty]
    private int _categoryId = 1;

    [ObservableProperty]
    private CategoryTreeDto? _selectedCategory;

    [ObservableProperty]
    private ObservableCollection<CategoryTreeDto> _categories = [];

    [ObservableProperty]
    private string _errorMessage = "";

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isSaving;

    [ObservableProperty]
    private bool _isNotSaving = true;

    public event Action? Saved;
    public event Action? Cancelled;
    public event Action<string>? PermissionDenied;

    public ItemEditViewModel(IItemService itemService, ICategoryService categoryService, IAuthService authService)
    {
        _itemService = itemService;
        _categoryService = categoryService;
        _authService = authService;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            if (_editingId is null)
                Article = await _itemService.GetNextArticleAsync();
            var cats = await _categoryService.GetTreeAsync();
            Categories = [.. cats];
            if (Categories.Count > 0 && _editingId is null)
                CategoryId = Categories[0].Id;
            SelectCategoryById();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void SelectCategoryById()
    {
        SelectedCategory = Categories.FirstOrDefault(c => c.Id == CategoryId);
    }

    partial void OnCategoryIdChanged(int value)
    {
        SelectCategoryById();
    }

    partial void OnSelectedCategoryChanged(CategoryTreeDto? value)
    {
        if (value is not null)
            CategoryId = value.Id;
    }

    public async Task LoadForEditAsync(int id)
    {
        IsLoading = true;
        try
        {
            var item = await _itemService.GetByIdAsync(id);
            if (item is null) return;

            _editingId = item.Id;
            PageTitle = "Редактирование товара";
            Name = item.Name;
            Article = item.Article;
            Unit = item.Unit;
            Price = item.Price;
            MinStock = item.MinStock;
            MaxStock = item.MaxStock;
            IsBatch = item.IsBatch;
            Barcode = item.Barcode;
            CategoryId = item.CategoryId;

            var cats = await _categoryService.GetTreeAsync();
            Categories = [.. cats];
            SelectCategoryById();
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var perm = _editingId.HasValue ? Permissions.ItemsEdit : Permissions.ItemsCreate;
        if (!await _authService.HasPermissionAsync(_authService.CurrentUser!, perm))
        {
            PermissionDenied?.Invoke("Нет прав на сохранение товара");
            return;
        }

        HasError = false;
        IsSaving = true;
        IsNotSaving = false;

        try
        {
            if (_editingId.HasValue)
            {
                await _itemService.UpdateAsync(_editingId.Value, new UpdateItemDto(
                    Name.Trim(), Article.Trim(), Unit.Trim(),
                    Price, MinStock, MaxStock, IsBatch,
                    string.IsNullOrWhiteSpace(Barcode) ? null : Barcode.Trim(),
                    CategoryId));
            }
            else
            {
                await _itemService.CreateAsync(new CreateItemDto(
                    Name.Trim(), Article.Trim(), Unit.Trim(),
                    Price, MinStock, MaxStock, IsBatch,
                    string.IsNullOrWhiteSpace(Barcode) ? null : Barcode.Trim(),
                    CategoryId));
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
            IsNotSaving = true;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        Cancelled?.Invoke();
    }
}
