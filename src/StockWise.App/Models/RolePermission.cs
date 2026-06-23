namespace StockWise.App.Models;

public class RolePermission
{
    public int Id { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Permission { get; set; } = string.Empty;
}
