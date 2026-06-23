namespace StockWise.App;

public class AppSettings
{
    public ConnectionStringsSection ConnectionStrings { get; set; } = new();

    public class ConnectionStringsSection
    {
        public string DefaultConnection { get; set; } = string.Empty;
    }
}
