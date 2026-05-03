using ErpSystem.Domain.Common;

namespace ErpSystem.Domain.Inventory;

public class UnitOfMeasure : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
