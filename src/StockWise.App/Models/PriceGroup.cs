using System.Collections.Generic;

namespace StockWise.App.Models;

public class PriceGroup
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<ItemPrice> ItemPrices { get; set; } = new();
}
