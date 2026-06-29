using System.Collections.Generic;

namespace StockWise.App.Models;



public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? INN { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public List<Order> Orders { get; set; } = new();
    public List<OutgoingInvoice> OutgoingInvoices { get; set; } = new();
    public List<Document> Documents { get; set; } = new();
}
