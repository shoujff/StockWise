namespace StockWise.App.Models;

public class OrderLine
{
    public int Id { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
    public decimal ShippedQty { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;
}
