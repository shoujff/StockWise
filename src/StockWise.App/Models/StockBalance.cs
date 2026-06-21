using System;
using System.Collections.Generic;

namespace StockWise.App.Models;

public class StockBalance
{
    public long Id { get; set; }
    public decimal Quantity { get; set; }
    public decimal ReservedQty { get; set; }
    public decimal Price { get; set; }
    public string? BatchNo { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public decimal AvailableQty => Quantity - ReservedQty;

    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    public List<Reservation> Reservations { get; set; } = new();
    public List<Transaction> Transactions { get; set; } = new();
}
