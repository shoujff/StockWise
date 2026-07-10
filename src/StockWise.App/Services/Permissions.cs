namespace StockWise.App.Services;

public static class Permissions
{
    public const string ItemsView = "Items.View";
    public const string ItemsCreate = "Items.Create";
    public const string ItemsEdit = "Items.Edit";
    public const string ItemsDelete = "Items.Delete";

    public const string WarehouseView = "Warehouse.View";
    public const string WarehouseCreate = "Warehouse.Create";
    public const string WarehouseEdit = "Warehouse.Edit";
    public const string WarehouseDelete = "Warehouse.Delete";

    public const string DocumentsView = "Documents.View";
    public const string DocumentsCreate = "Documents.Create";
    public const string DocumentsPost = "Documents.Post";
    public const string DocumentsCancel = "Documents.Cancel";

    public const string OrdersView = "Orders.View";
    public const string OrdersCreate = "Orders.Create";
    public const string OrdersEdit = "Orders.Edit";

    public const string InventoryCreate = "Inventory.Create";
    public const string InventoryEdit = "Inventory.Edit";

    public const string ReportsView = "Reports.View";

    public const string AdminUsers = "Admin.Users";
    public const string AdminUsersCreate = "Admin.Users.Create";
    public const string AdminUsersEdit = "Admin.Users.Edit";
    public const string AdminUsersDelete = "Admin.Users.Delete";
    public const string AdminCustomers = "Admin.Customers";
    public const string AdminCustomersCreate = "Admin.Customers.Create";
    public const string AdminCustomersEdit = "Admin.Customers.Edit";
    public const string AdminCustomersDelete = "Admin.Customers.Delete";
}
