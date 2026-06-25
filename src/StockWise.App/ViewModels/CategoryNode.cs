using System.Collections.ObjectModel;

namespace StockWise.App.ViewModels;

public class CategoryNode
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int? ParentId { get; set; }
    public int SortOrder { get; set; }
    public int Level { get; set; }
    public ObservableCollection<CategoryNode> Children { get; set; } = [];
    public bool HasItems { get; set; }
}
