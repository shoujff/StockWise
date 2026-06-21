namespace StockWise.App.Models;

public class InventoryLine
{
    public int Id { get; set; }
    public decimal ExpectedQty { get; set; }
    public decimal ActualQty { get; set; }
    public decimal Diff { get; set; }
    public decimal Price { get; set; }
    public string? BatchNo { get; set; }

    public int InventoryId { get; set; }
    public Inventory Inventory { get; set; } = null!;

    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;
}
