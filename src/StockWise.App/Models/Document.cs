using System;
using System.Collections.Generic;

namespace StockWise.App.Models;

public class Document
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Status { get; set; } = "Draft";
    public decimal TotalAmount { get; set; }

    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public string? SupplierName { get; set; }

    public int? FromWarehouseId { get; set; }
    public Warehouse? FromWarehouse { get; set; }

    public int? ToWarehouseId { get; set; }
    public Warehouse? ToWarehouse { get; set; }

    public int? StockRefDocId { get; set; }

    public int CreatedBy { get; set; }
    public User User { get; set; } = null!;

    public List<DocumentLine> Lines { get; set; } = new();
}
