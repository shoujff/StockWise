using System;

namespace StockWise.App.Models;

public class ItemPrice
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public DateOnly ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }

    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;
}
