using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockWise.App.Services;

namespace StockWise.App.ViewModels;

public partial class CustomerListViewModel : ObservableObject
{
    private readonly ICustomerService _customerService;
    private readonly IAuthService _authService;

    [ObservableProperty]
    private ObservableCollection<CustomerDto> _customers = [];

    [ObservableProperty]
    private string _searchTerm = "";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasItems;

    [ObservableProperty]
    private bool _hasNoItems = true;

    public event Action? CreateRequested;
    public event Action<int>? EditRequested;
    public event Action? CustomerDeleted;
    public event Action<string>? PermissionDenied;

    public CustomerListViewModel(ICustomerService customerService, IAuthService authService)
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
            var results = await _customerService.SearchAsync(
                string.IsNullOrWhiteSpace(SearchTerm) ? null : SearchTerm);
            Customers = [.. results];
            HasItems = Customers.Count > 0;
            HasNoItems = Customers.Count == 0;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DeleteAsync(int id)
    {
        if (!await _authService.HasPermissionAsync(_authService.CurrentUser!, Permissions.AdminCustomersDelete))
        {
            PermissionDenied?.Invoke("Нет прав на удаление контрагентов");
            return;
        }

        var success = await _customerService.DeleteAsync(id);
        if (success)
        {
            var item = Customers.FirstOrDefault(c => c.Id == id);
            if (item is not null) Customers.Remove(item);
            HasItems = Customers.Count > 0;
            HasNoItems = Customers.Count == 0;
            CustomerDeleted?.Invoke();
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
}
