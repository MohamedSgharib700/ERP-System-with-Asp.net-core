using ErpSystem.Domain.Common;
using ErpSystem.Domain.Inventory;

namespace ErpSystem.Domain.Purchasing;

public class PurchaseOrderItem : BaseEntity
{
    public int PurchaseOrderId { get; set; }
    public PurchaseOrder? PurchaseOrder { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public decimal Quantity { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxRate { get; set; }
    public decimal LineTotal => (Quantity * UnitCost) - Discount + ((Quantity * UnitCost - Discount) * TaxRate / 100);
}
