using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockWise.App.Services;

namespace StockWise.App.ViewModels;

public partial class CustomerEditViewModel : ObservableObject
{
    private readonly ICustomerService _customerService;
    private readonly IAuthService _authService;
    private int? _editingId;

    [ObservableProperty]
    private string _pageTitle = "Новый контрагент";

    [ObservableProperty]
    private string _name = "";

    [ObservableProperty]
    private string? _inn;

    [ObservableProperty]
    private string? _contactPerson;

    [ObservableProperty]
    private string? _phone;

    [ObservableProperty]
    private string? _email;

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

    public CustomerEditViewModel(ICustomerService customerService, IAuthService authService)
    {
        _customerService = customerService;
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
                var customer = await _customerService.GetByIdAsync(_editingId.Value);
                if (customer is not null)
                {
                    Name = customer.Name;
                    Inn = customer.INN;
                    ContactPerson = customer.ContactPerson;
                    Phone = customer.Phone;
                    Email = customer.Email;
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
        PageTitle = "Редактирование контрагента";
        await LoadAsync();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var perm = _editingId.HasValue ? Permissions.AdminCustomersEdit : Permissions.AdminCustomersCreate;
        if (!await _authService.HasPermissionAsync(_authService.CurrentUser!, perm))
        {
            PermissionDenied?.Invoke("Нет прав на сохранение контрагента");
            return;
        }

        HasError = false;
        IsSaving = true;
        IsNotSaving = false;

        try
        {
            if (_editingId.HasValue)
            {
                await _customerService.UpdateAsync(_editingId.Value, new UpdateCustomerDto(
                    Name.Trim(), Inn?.Trim(), ContactPerson?.Trim(), Phone?.Trim(), Email?.Trim()));
            }
            else
            {
                await _customerService.CreateAsync(new CreateCustomerDto(
                    Name.Trim(), Inn?.Trim(), ContactPerson?.Trim(), Phone?.Trim(), Email?.Trim()));
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
