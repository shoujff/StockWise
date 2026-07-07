using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockWise.App.Services;

namespace StockWise.App.ViewModels;

public partial class InventoryLineEditDto : ObservableObject
{
    [ObservableProperty] private int _id;
    [ObservableProperty] private int _itemId;
    [ObservableProperty] private string _itemName = "";
    [ObservableProperty] private string _article = "";
    [ObservableProperty] private string _unit = "";
    [ObservableProperty] private decimal _expectedQty;
    [ObservableProperty] private decimal _actualQty;
    [ObservableProperty] private decimal _diff;
    [ObservableProperty] private decimal _price;
    [ObservableProperty] private string? _batchNo;

    partial void OnActualQtyChanged(decimal value) => Diff = value - ExpectedQty;
}

public partial class InventoryEditViewModel : ObservableObject
{
    private readonly IInventoryService _inventoryService;
    private readonly IAuthService _authService;
    private int? _editingId;

    [ObservableProperty]
    private string _pageTitle = "Новая инвентаризация";

    [ObservableProperty]
    private string _inventoryNumber = "";

    [NotifyPropertyChangedFor(nameof(IsDraft))]
    [NotifyPropertyChangedFor(nameof(IsConfirmed))]
    [NotifyPropertyChangedFor(nameof(IsCancelled))]
    [NotifyPropertyChangedFor(nameof(CanEdit))]
    [ObservableProperty]
    private string _status = "";

    [ObservableProperty]
    private bool _hasInventory;

    [ObservableProperty]
    private string _warehouseName = "";

    [ObservableProperty]
    private string _date = "";

    public ObservableCollection<InventoryLineEditDto> Lines { get; } = [];

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isSaving;

    [ObservableProperty]
    private string _errorMessage = "";

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private decimal _totalDiff;

    [ObservableProperty]
    private int _matchedCount;

    [ObservableProperty]
    private int _surplusCount;

    [ObservableProperty]
    private int _shortageCount;

    public bool IsDraft => Status == "Draft";
    public bool IsConfirmed => Status == "Confirmed";
    public bool IsCancelled => Status == "Cancelled";
    public bool CanEdit => Status is "" or "Draft";

    public event Action? Saved;
    public event Action? Cancelled;
    public event Action<string>? PermissionDenied;

    public InventoryEditViewModel(IInventoryService inventoryService, IAuthService authService)
    {
        _inventoryService = inventoryService;
        _authService = authService;
    }

    private int CurrentUserId => _authService.CurrentUser?.Id ?? 0;

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            if (_editingId is null)
            {
                InventoryNumber = await _inventoryService.GenerateNumberAsync();
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task LoadForViewAsync(int id)
    {
        _editingId = id;
        HasInventory = true;
        var inventory = await _inventoryService.GetByIdAsync(id);
        if (inventory is null) return;

        BindInventory(inventory);
        await LoadAsync();
    }

    public async Task LoadForCreateAsync(int warehouseId)
    {
        HasInventory = true;
        try
        {
            IsLoading = true;
            var inventory = await _inventoryService.CreateAsync(warehouseId, CurrentUserId);
            _editingId = inventory.Id;
            BindInventory(inventory);
            PageTitle = $"Инвентаризация #{inventory.Number}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void BindInventory(InventoryDetailDto inventory)
    {
        InventoryNumber = inventory.Number;
        Status = inventory.Status;
        WarehouseName = inventory.WarehouseName;
        Date = inventory.Date.ToString("dd.MM.yyyy HH:mm");
        PageTitle = $"Инвентаризация #{inventory.Number}";

        Lines.Clear();
        foreach (var l in inventory.Lines)
        {
            Lines.Add(new InventoryLineEditDto
            {
                Id = l.Id,
                ItemId = l.ItemId,
                ItemName = l.ItemName ?? "",
                Article = l.Article ?? "",
                Unit = l.Unit ?? "",
                ExpectedQty = l.ExpectedQty,
                ActualQty = l.ActualQty,
                Diff = l.Diff,
                Price = l.Price,
                BatchNo = l.BatchNo
            });
        }

        RecalculateStats();
    }

    private void RecalculateStats()
    {
        TotalDiff = Lines.Sum(l => Math.Abs(l.Diff));
        MatchedCount = Lines.Count(l => l.Diff == 0);
        SurplusCount = Lines.Count(l => l.Diff > 0);
        ShortageCount = Lines.Count(l => l.Diff < 0);
    }

    [RelayCommand]
    private async Task SaveActualQty(InventoryLineEditDto line)
    {
        if (!CanEdit) return;

        HasError = false;
        try
        {
            await _inventoryService.UpdateActualQtyAsync(line.Id, line.ActualQty);
            line.Diff = line.ActualQty - line.ExpectedQty;
            RecalculateStats();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
    }

    [RelayCommand]
    private async Task ConfirmAsync()
    {
        if (_editingId is null) return;

        if (!await _authService.HasPermissionAsync(_authService.CurrentUser!, Permissions.InventoryEdit))
        {
            PermissionDenied?.Invoke("Нет прав на подтверждение инвентаризации");
            return;
        }

        HasError = false;
        IsSaving = true;

        try
        {
            var result = await _inventoryService.ConfirmAsync(_editingId.Value, CurrentUserId);
            if (!result.Success)
            {
                ErrorMessage = result.Error ?? "Ошибка при подтверждении";
                HasError = true;
                return;
            }

            Status = "Confirmed";

            var updated = await _inventoryService.GetByIdAsync(_editingId.Value);
            if (updated is not null)
                BindInventory(updated);

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
        }
    }

    [RelayCommand]
    private async Task CancelInventoryAsync()
    {
        if (_editingId is null) return;

        HasError = false;
        IsSaving = true;

        try
        {
            var result = await _inventoryService.CancelAsync(_editingId.Value);
            if (!result.Success)
            {
                ErrorMessage = result.Error ?? "Ошибка при отмене";
                HasError = true;
                return;
            }

            Status = "Cancelled";
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
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        Cancelled?.Invoke();
    }
}
