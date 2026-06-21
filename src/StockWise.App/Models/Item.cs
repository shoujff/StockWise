using System.Collections.Generic;
using System;
using System.Linq;

namespace StockWise.App.Models;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Article { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal MinStock { get; set; }
    public decimal MaxStock { get; set; }
    public bool IsBatch { get; set; }
    public string? Barcode { get; set; }
    public string? ImagePath { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public List<StockBalance> StockBalances { get; set; } = new();
    public List<ItemPrice> Prices { get; set; } = new();
    public List<Transaction> Transactions { get; set; } = new();
}