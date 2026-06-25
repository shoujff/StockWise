using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockWise.App.Services;

namespace StockWise.App.ViewModels;

public partial class CategoryListViewModel : ObservableObject
{
    private readonly ICategoryService _categoryService;

    [ObservableProperty]
    private ObservableCollection<CategoryNode> _roots = [];

    [ObservableProperty]
    private CategoryNode? _selectedNode;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _headerText = "";

    [ObservableProperty]
    private string _editName = "";

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private bool _isCreating;

    [ObservableProperty]
    private int? _editingId;

    [ObservableProperty]
    private string _errorMessage = "";

    [ObservableProperty]
    private bool _hasError;

    public CategoryListViewModel(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var tree = await _categoryService.GetTreeAsync();
            Roots = [.. BuildTree(tree)];
        }
        finally
        {
            IsLoading = false;
        }
    }

    private static ObservableCollection<CategoryNode> BuildTree(
        System.Collections.Generic.IEnumerable<CategoryTreeDto> flat)
    {
        var nodes = flat.Select(dto => new CategoryNode
        {
            Id = dto.Id,
            Name = dto.Name,
            ParentId = dto.ParentId,
            SortOrder = dto.SortOrder,
            Level = dto.Level,
            HasItems = dto.HasChildren
        }).ToList();

        var roots = new ObservableCollection<CategoryNode>();
        var lookup = nodes.ToLookup(n => n.ParentId);

        foreach (var node in nodes)
        {
            if (node.ParentId is null)
            {
                roots.Add(node);
            }
            else
            {
                var parent = nodes.FirstOrDefault(n => n.Id == node.ParentId.Value);
                if (parent is not null)
                    parent.Children.Add(node);
            }
        }

        return roots;
    }

    partial void OnSelectedNodeChanged(CategoryNode? value)
    {
        if (value is not null && !IsCreating)
        {
            EditName = value.Name;
            EditingId = value.Id;
            IsEditing = true;
        }
    }

    [RelayCommand]
    private void StartCreate()
    {
        IsCreating = true;
        IsEditing = false;
        EditName = "";
        EditingId = null;
        HasError = false;
        HeaderText = "Новая категория";
    }

    [RelayCommand]
    private void CancelEdit()
    {
        IsCreating = false;
        IsEditing = false;
        EditName = "";
        EditingId = null;
        HasError = false;
        SelectedNode = null;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        HasError = false;

        if (string.IsNullOrWhiteSpace(EditName))
        {
            ErrorMessage = "Название категории не может быть пустым";
            HasError = true;
            return;
        }

        try
        {
            if (IsCreating)
            {
                await _categoryService.CreateAsync(new CreateCategoryDto(
                    EditName.Trim(), SelectedNode?.Id));
            }
            else if (EditingId.HasValue)
            {
                var current = await _categoryService.GetByIdAsync(EditingId.Value);
                if (current is null) return;

                await _categoryService.UpdateAsync(EditingId.Value, new UpdateCategoryDto(
                    EditName.Trim(), current.ParentId, current.SortOrder));
            }

            IsCreating = false;
            IsEditing = false;
            EditName = "";
            EditingId = null;
            SelectedNode = null;

            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
    }

    [RelayCommand]
    private async Task DeleteAsync(int id)
    {
        try
        {
            var success = await _categoryService.DeleteAsync(id);
            if (!success)
            {
                ErrorMessage = "Нельзя удалить категорию — у неё есть подкатегории или товары";
                HasError = true;
                return;
            }

            IsEditing = false;
            SelectedNode = null;
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
    }
}
