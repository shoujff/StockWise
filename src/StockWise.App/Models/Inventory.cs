using System;
using System.Collections.Generic;

namespace StockWise.App.Models;

public class Inventory
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalDiff { get; set; }
    public int CreatedBy { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public Warehouse Warehouse { get; set; } = null!;

    public List<InventoryLine> Lines { get; set; } = new();
}
