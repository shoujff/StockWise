using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockWise.App.Services;

namespace StockWise.App.ViewModels;

public partial class WarehouseEditViewModel : ObservableObject
{
    private readonly IWarehouseService _warehouseService;
    private readonly IAuthService _authService;
    private int? _editingId;

    [ObservableProperty]
    private string _pageTitle = "Новый склад";

    [ObservableProperty]
    private string _name = "";

    [ObservableProperty]
    private string? _address;

    [ObservableProperty]
    private bool _isActive = true;

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

    public WarehouseEditViewModel(IWarehouseService warehouseService, IAuthService authService)
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
            if (_editingId.HasValue)
            {
                var warehouse = await _warehouseService.GetByIdAsync(_editingId.Value);
                if (warehouse is not null)
                {
                    Name = warehouse.Name;
                    Address = warehouse.Address;
                    IsActive = warehouse.IsActive;
                }
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
        PageTitle = "Редактирование склада";
        await LoadAsync();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var perm = _editingId.HasValue ? Permissions.WarehouseEdit : Permissions.WarehouseCreate;
        if (!await _authService.HasPermissionAsync(_authService.CurrentUser!, perm))
        {
            PermissionDenied?.Invoke("Нет прав на сохранение склада");
            return;
        }

        HasError = false;
        IsSaving = true;
        IsNotSaving = false;

        try
        {
            if (_editingId.HasValue)
            {
                await _warehouseService.UpdateAsync(_editingId.Value, new UpdateWarehouseDto(
                    Name.Trim(), Address?.Trim(), IsActive));
            }
            else
            {
                await _warehouseService.CreateAsync(new CreateWarehouseDto(
                    Name.Trim(), Address?.Trim()));
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
