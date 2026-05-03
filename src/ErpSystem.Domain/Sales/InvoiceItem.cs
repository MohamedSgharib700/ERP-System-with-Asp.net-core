using ErpSystem.Domain.Common;
using ErpSystem.Domain.Inventory;

namespace ErpSystem.Domain.Sales;

public class InvoiceItem : BaseEntity
{
    public int InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public string? Description { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxRate { get; set; }
    public decimal LineTotal => (Quantity * UnitPrice) - Discount + ((Quantity * UnitPrice - Discount) * TaxRate / 100);
}
