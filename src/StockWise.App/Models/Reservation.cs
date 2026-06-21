using System;

namespace StockWise.App.Models;

public class Reservation
{
    public long Id { get; set; }
    public decimal Quantity { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public long StockBalanceId { get; set; }
    public StockBalance StockBalance { get; set; } = null!;

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
}
