using System;
using System.Collections.Generic;

namespace StockWise.App.Models;

public class OutgoingInvoice
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public int CreatedBy { get; set; }
    public User User { get; set; } = null!;

    public List<OutgoingLine> Lines { get; set; } = new();
}
