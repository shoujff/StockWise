using System;

namespace StockWise.App.Models;

public class Transaction
{
    public long Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Direction { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? BatchNo { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int? RefDocId { get; set; }
    public string? RefDocType { get; set; }

    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    public int CreatedBy { get; set; }
    public User User { get; set; } = null!;
}