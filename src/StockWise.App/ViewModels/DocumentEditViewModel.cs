using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockWise.App.Services;

namespace StockWise.App.ViewModels;

public partial class DocumentLineEditDto : ObservableObject
{
    [ObservableProperty] private int _itemId;
    [ObservableProperty] private string _itemName = "";
    [ObservableProperty] private string _article = "";
    [ObservableProperty] private string _unit = "";
    [ObservableProperty] private decimal _quantity = 1;
    [ObservableProperty] private decimal _price;
    [ObservableProperty] private decimal _amount;
    [ObservableProperty] private string? _batchNo;
    [ObservableProperty] private DateOnly? _expiryDate;
    [ObservableProperty] private bool _isBatch;

    partial void OnQuantityChanged(decimal value) => Amount = value * Price;
    partial void OnPriceChanged(decimal value) => Amount = value * Quantity;
}

public partial class DocumentEditViewModel : ObservableObject
{
    private readonly IDocumentService _documentService;
    private readonly IItemService _itemService;
    private readonly IWarehouseService _warehouseService;
    private int? _editingId;

    [ObservableProperty]
    private string _pageTitle = "Новый документ";

    [ObservableProperty]
    private string _documentType = "";

    [ObservableProperty]
    private string _documentNumber = "";

    [ObservableProperty]
    private string _status = "Draft";

    [ObservableProperty]
    private string? _supplierName;

    [ObservableProperty]
    private int? _customerId;

    [ObservableProperty]
    private WarehouseOptionDto? _selectedFromWarehouse;

    [ObservableProperty]
    private WarehouseOptionDto? _selectedToWarehouse;

    [ObservableProperty]
    private ObservableCollection<WarehouseOptionDto> _warehouses = [];

    public ObservableCollection<DocumentLineEditDto> Lines { get; } = [];

    [ObservableProperty]
    private string _searchItemTerm = "";

    [ObservableProperty]
    private ObservableCollection<ItemDto> _searchResults = [];

    [ObservableProperty]
    private bool _hasSearchResults;

    [ObservableProperty]
    private ItemDto? _selectedSearchItem;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isSaving;

    [ObservableProperty]
    private string _errorMessage = "";

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private decimal _totalAmount;

    public bool IsIncome => DocumentType == "Income";
    public bool IsOutcome => DocumentType == "Outcome";
    public bool IsTransfer => DocumentType == "Transfer";
    public bool IsDraft => Status == "Draft";
    public bool IsPosted => Status == "Posted";

    private async Task<Dictionary<int, bool>> GetItemBatchFlagsAsync(IEnumerable<int> itemIds)
    {
        var ids = itemIds.Distinct().ToList();
        var items = await _itemService.GetAllAsync();
        return items.Where(i => ids.Contains(i.Id)).ToDictionary(i => i.Id, i => i.IsBatch);
    }

    public event Action? Saved;
    public event Action? Cancelled;

    public DocumentEditViewModel(
        IDocumentService documentService,
        IItemService itemService,
        IWarehouseService warehouseService)
    {
        _documentService = documentService;
        _itemService = itemService;
        _warehouseService = warehouseService;
    }

    public void Initialize(string type)
    {
        DocumentType = type;
        PageTitle = type switch
        {
            "Income" => "Новый приход",
            "Outcome" => "Новый расход",
            "Transfer" => "Новое перемещение",
            _ => "Новый документ"
        };
        OnPropertyChanged(nameof(IsIncome));
        OnPropertyChanged(nameof(IsOutcome));
        OnPropertyChanged(nameof(IsTransfer));
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var whs = await _warehouseService.GetAllAsync();
            Warehouses = [.. whs.Select(w => new WarehouseOptionDto(w.Id, w.Name))];

            if (_editingId is null)
            {
                DocumentNumber = await _documentService.GenerateNumberAsync(DocumentType);
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
        var doc = await _documentService.GetByIdAsync(id);
        if (doc is null) return;

        DocumentType = doc.Type;
        DocumentNumber = doc.Number;
        Status = doc.Status;
        SupplierName = doc.SupplierName;
        CustomerId = doc.CustomerId;
        TotalAmount = doc.TotalAmount;

        PageTitle = doc.Type switch
        {
            "Income" => $"Приход #{doc.Number}",
            "Outcome" => $"Расход #{doc.Number}",
            "Transfer" => $"Перемещение #{doc.Number}",
            _ => $"Документ #{doc.Number}"
        };

        Lines.Clear();

        var batchFlags = await GetItemBatchFlagsAsync(doc.Lines.Select(l => l.ItemId));
        foreach (var l in doc.Lines)
        {
            Lines.Add(new DocumentLineEditDto
            {
                ItemId = l.ItemId,
                ItemName = l.ItemName ?? "",
                Article = l.Article ?? "",
                Unit = l.Unit ?? "",
                Quantity = l.Quantity,
                Price = l.Price,
                Amount = l.Amount,
                BatchNo = l.BatchNo,
                ExpiryDate = l.ExpiryDate,
                IsBatch = batchFlags.TryGetValue(l.ItemId, out var b) && b
            });
        }

        RecalculateTotal();

        OnPropertyChanged(nameof(IsIncome));
        OnPropertyChanged(nameof(IsOutcome));
        OnPropertyChanged(nameof(IsTransfer));
        OnPropertyChanged(nameof(IsDraft));
        OnPropertyChanged(nameof(IsPosted));

        await LoadAsync();

        SelectedFromWarehouse = doc.FromWarehouseId.HasValue
            ? Warehouses.FirstOrDefault(w => w.Id == doc.FromWarehouseId.Value)
            : null;
        SelectedToWarehouse = doc.ToWarehouseId.HasValue
            ? Warehouses.FirstOrDefault(w => w.Id == doc.ToWarehouseId.Value)
            : null;
    }

    [RelayCommand]
    private async Task SearchItemAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchItemTerm))
        {
            SearchResults = [];
            HasSearchResults = false;
            return;
        }

        var results = await _itemService.SearchAsync(SearchItemTerm, null);
        SearchResults = [.. results];
        HasSearchResults = SearchResults.Count > 0;
    }

    partial void OnSearchItemTermChanged(string value)
    {
        _ = SearchItemAsync();
    }

    partial void OnSelectedSearchItemChanged(ItemDto? value)
    {
        if (value is null) return;
        AddLineFromItem(value);
    }

    [RelayCommand]
    private void AddLine()
    {
        if (SelectedSearchItem is null) return;
        AddLineFromItem(SelectedSearchItem);
    }

    private void AddLineFromItem(ItemDto item)
    {
        if (Lines.Any(l => l.ItemId == item.Id))
            return;

        Lines.Add(new DocumentLineEditDto
        {
            ItemId = item.Id,
            ItemName = item.Name,
            Article = item.Article,
            Unit = item.Unit,
            Quantity = 1,
            Price = item.Price,
            Amount = item.Price,
            IsBatch = item.IsBatch
        });

        SelectedSearchItem = null;
        SearchItemTerm = "";
        SearchResults = [];
        HasSearchResults = false;
        RecalculateTotal();
    }

    [RelayCommand]
    private void RemoveLine(DocumentLineEditDto line)
    {
        Lines.Remove(line);
        RecalculateTotal();
    }

    private void RecalculateTotal()
    {
        TotalAmount = Lines.Sum(l => l.Amount);
    }

    private void ValidateLines()
    {
        foreach (var line in Lines)
        {
            if (line.IsBatch && string.IsNullOrWhiteSpace(line.BatchNo))
                throw new ArgumentException(
                    $"Для товара \"{line.ItemName}\" (партионный учёт) укажите номер партии");
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        HasError = false;
        IsSaving = true;

        try
        {
            if (Lines.Count == 0)
                throw new ArgumentException("Добавьте хотя бы одну строку");
            ValidateLines();

            var fromId = SelectedFromWarehouse?.Id;
            var toId = SelectedToWarehouse?.Id;

            if (DocumentType == "Income" && toId is null)
                throw new ArgumentException("Выберите склад");
            if (DocumentType == "Outcome" && fromId is null)
                throw new ArgumentException("Выберите склад");
            if (DocumentType == "Transfer")
            {
                if (fromId is null)
                    throw new ArgumentException("Выберите склад отправления");
                if (toId is null)
                    throw new ArgumentException("Выберите склад назначения");
                if (fromId == toId)
                    throw new ArgumentException("Склады должны отличаться");
            }

            if (_editingId.HasValue)
            {
                var doc = await _documentService.GetByIdAsync(_editingId.Value);
                if (doc is null) return;

                if (doc.Status != "Draft")
                    throw new InvalidOperationException("Нельзя редактировать проведённый документ");

                await _documentService.DeleteAsync(_editingId.Value);
            }

            var createLines = Lines.Select(l => new CreateDocumentLineDto(
                l.ItemId, l.Quantity, l.Price, l.BatchNo, l.ExpiryDate)).ToList();

            var dto = new CreateDocumentDto(
                DocumentType, CustomerId, SupplierName,
                DocumentType == "Transfer" || DocumentType == "Outcome" ? fromId : null,
                DocumentType == "Transfer" || DocumentType == "Income" ? toId : null,
                createLines);

            await _documentService.CreateAsync(dto, 1);

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
    private async Task PostAsync()
    {
        if (_editingId is null) return;

        HasError = false;
        IsSaving = true;

        try
        {
            ValidateLines();
            var (success, error) = await _documentService.PostAsync(_editingId.Value, 1);
            if (success)
            {
                Status = "Posted";
                OnPropertyChanged(nameof(IsDraft));
                OnPropertyChanged(nameof(IsPosted));
                Saved?.Invoke();
            }
            else
            {
                ErrorMessage = error ?? "Ошибка при проведении";
                HasError = true;
            }
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
    private async Task CancelDocAsync()
    {
        if (_editingId is null) return;

        HasError = false;
        IsSaving = true;

        try
        {
            var (success, error) = await _documentService.CancelAsync(_editingId.Value, 1);
            if (success)
            {
                Status = "Cancelled";
                OnPropertyChanged(nameof(IsDraft));
                OnPropertyChanged(nameof(IsPosted));
                Saved?.Invoke();
            }
            else
            {
                ErrorMessage = error ?? "Ошибка при отмене";
                HasError = true;
            }
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
