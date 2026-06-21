using System;
using System.Collections.Generic;

namespace StockWise.App.Models;

public class Order
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public int CreatedBy { get; set; }
    public User User { get; set; } = null!;

    public List<OrderLine> Lines { get; set; } = new();
    public List<Reservation> Reservations { get; set; } = new();
}
