using System;

namespace StockWise.App.Models;

public class DocumentLine
{
    public int Id { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
    public string? BatchNo { get; set; }
    public DateOnly? ExpiryDate { get; set; }

    public int DocumentId { get; set; }
    public Document Document { get; set; } = null!;

    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;
}
