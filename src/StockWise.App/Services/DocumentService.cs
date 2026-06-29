using Microsoft.EntityFrameworkCore;
using StockWise.App.Models;
using StockWise.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockWise.App.Services;

public class DocumentService : IDocumentService
{
    private readonly StockDb _db;
    private readonly IStockCoreService _stockCore;
    private readonly IItemService _itemService;

    public DocumentService(StockDb db, IStockCoreService stockCore, IItemService itemService)
    {
        _db = db;
        _stockCore = stockCore;
        _itemService = itemService;
    }

    public async Task<IEnumerable<DocumentListDto>> GetAllAsync(
        string? typeFilter = null, string? statusFilter = null)
    {
        var query = _db.Documents.AsQueryable();

        if (!string.IsNullOrWhiteSpace(typeFilter))
            query = query.Where(d => d.Type == typeFilter);

        if (!string.IsNullOrWhiteSpace(statusFilter))
            query = query.Where(d => d.Status == statusFilter);

        return await query
            .OrderByDescending(d => d.Date)
            .ThenByDescending(d => d.Id)
            .Select(d => new DocumentListDto(
                d.Id, d.Type, d.Number, d.Date, d.Status,
                d.TotalAmount, d.Lines.Count))
            .ToListAsync();
    }

    public async Task<DocumentDetailDto?> GetByIdAsync(int id)
    {
        return await _db.Documents
            .Include(d => d.Lines)
            .ThenInclude(l => l.Item)
            .Include(d => d.Customer)
            .Include(d => d.FromWarehouse)
            .Include(d => d.ToWarehouse)
            .Where(d => d.Id == id)
            .Select(d => new DocumentDetailDto(
                d.Id, d.Type, d.Number, d.Date, d.Status, d.TotalAmount,
                d.CustomerId, d.Customer!.Name, d.SupplierName,
                d.FromWarehouseId, d.FromWarehouse!.Name,
                d.ToWarehouseId, d.ToWarehouse!.Name,
                d.Lines.Select(l => new DocumentLineDto(
                    l.Id, l.ItemId, l.Item.Name, l.Item.Article, l.Item.Unit,
                    l.Quantity, l.Price, l.Amount, l.BatchNo, l.ExpiryDate
                )).ToList()))
            .FirstOrDefaultAsync();
    }

    public async Task<DocumentDetailDto> CreateAsync(CreateDocumentDto dto, int userId)
    {
        if (dto.Lines.Count == 0)
            throw new ArgumentException("Документ должен содержать хотя бы одну строку");

        var doc = new Document
        {
            Type = dto.Type,
            Number = await GenerateNumberAsync(dto.Type),
            Date = DateTime.UtcNow,
            Status = "Draft",
            CustomerId = dto.Type == "Outcome" ? dto.CustomerId : null,
            SupplierName = dto.Type == "Income" ? dto.SupplierName : null,
            FromWarehouseId = dto.Type == "Outcome" || dto.Type == "Transfer" ? dto.FromWarehouseId : null,
            ToWarehouseId = dto.Type == "Income" || dto.Type == "Transfer" ? dto.ToWarehouseId : null,
            CreatedBy = userId
        };

        foreach (var line in dto.Lines)
        {
            var amount = line.Quantity * line.Price;
            doc.Lines.Add(new DocumentLine
            {
                ItemId = line.ItemId,
                Quantity = line.Quantity,
                Price = line.Price,
                Amount = amount,
                BatchNo = line.BatchNo,
                ExpiryDate = line.ExpiryDate
            });
            doc.TotalAmount += amount;
        }

        _db.Documents.Add(doc);
        await _db.SaveChangesAsync();

        return (await GetByIdAsync(doc.Id))!;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var doc = await _db.Documents
            .Include(d => d.Lines)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (doc is null || doc.Status != "Draft")
            return false;

        _db.DocumentLines.RemoveRange(doc.Lines);
        _db.Documents.Remove(doc);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<(bool Success, string? Error)> PostAsync(int id, int userId)
    {
        var doc = await _db.Documents
            .Include(d => d.Lines)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (doc is null)
            return (false, "Документ не найден");
        if (doc.Status != "Draft")
            return (false, $"Нельзя провести документ со статусом \"{doc.Status}\"");

        var lines = doc.Lines.Select(l => new StockLineDto(
            l.ItemId, l.Quantity, l.Price, l.BatchNo, l.ExpiryDate));

        PostResult result;

        result = doc.Type switch
        {
            "Income" => await _stockCore.PostIncomeAsync(
                doc.ToWarehouseId!.Value, lines, userId, "Document"),
            "Outcome" => await _stockCore.PostOutcomeAsync(
                doc.FromWarehouseId!.Value, lines, userId, "Document"),
            "Transfer" => await _stockCore.PostTransferAsync(
                doc.FromWarehouseId!.Value, doc.ToWarehouseId!.Value, lines, userId),
            _ => new PostResult(false, $"Неизвестный тип документа: {doc.Type}")
        };

        if (!result.Success)
            return (false, result.ErrorMessage);

        doc.Status = "Posted";
        doc.StockRefDocId = result.RefDocId;
        await _db.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> CancelAsync(int id, int userId)
    {
        var doc = await _db.Documents.FindAsync(id);

        if (doc is null)
            return (false, "Документ не найден в системе");
        if (doc.Status != "Posted")
            return (false, "Отменить можно только проведённый документ");
        if (doc.StockRefDocId is null)
            return (false, "У документа нет проводок для отмены");

        var result = await _stockCore.CancelDocumentAsync("Document", doc.StockRefDocId.Value, userId);

        if (!result.Success)
            return (false, result.ErrorMessage);

        doc.Status = "Cancelled";
        await _db.SaveChangesAsync();

        return (true, null);
    }

    public async Task<string> GenerateNumberAsync(string type)
    {
        var prefix = type switch
        {
            "Income" => "IN",
            "Outcome" => "OUT",
            "Transfer" => "TRF",
            _ => "DOC"
        };

        var year = DateTime.UtcNow.Year;
        var pattern = $"{prefix}-{year}-";

        var lastNumber = await _db.Documents
            .Where(d => d.Type == type && d.Number.StartsWith(pattern))
            .OrderByDescending(d => d.Id)
            .Select(d => d.Number)
            .FirstOrDefaultAsync();

        int seq = 1;
        if (lastNumber is not null)
        {
            var parts = lastNumber.Split('-');
            if (parts.Length > 0 && int.TryParse(parts[^1], out var lastSeq))
                seq = lastSeq + 1;
        }

        return $"{pattern}{seq:D4}";
    }
}
