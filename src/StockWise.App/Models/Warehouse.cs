using System.Collections.Generic;

namespace StockWise.App.Models;

public class Warehouse
{
     public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;

    public List<StockBalance> StockBalances { get; set; } = new();
    public List<Transaction> Transactions { get; set; } = new();
    public List<Inventory> Inventories { get; set; } = new();

}