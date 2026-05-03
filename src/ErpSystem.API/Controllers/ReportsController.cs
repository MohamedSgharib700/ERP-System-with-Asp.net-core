using ErpSystem.Application.Interfaces;
using ErpSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IApplicationDbContext _db;
    public ReportsController(IApplicationDbContext db) => _db = db;

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var now = DateTime.UtcNow;
        var startMonth = new DateTime(now.Year, now.Month, 1);

        return Ok(new
        {
            Employees = await _db.Employees.CountAsync(e => e.Status == EmployeeStatus.Active),
            Customers = await _db.Customers.CountAsync(c => c.IsActive),
            Suppliers = await _db.Suppliers.CountAsync(s => s.IsActive),
            Products = await _db.Products.CountAsync(p => p.IsActive),
            LowStockProducts = await _db.Products.CountAsync(p => p.IsActive && p.QuantityOnHand <= p.ReorderLevel),
            OpenSalesOrders = await _db.SalesOrders.CountAsync(o => o.Status != SalesOrderStatus.Delivered && o.Status != SalesOrderStatus.Cancelled),
            UnpaidInvoices = await _db.Invoices.CountAsync(i => i.Status != InvoiceStatus.Paid && i.Status != InvoiceStatus.Cancelled),
            MonthlySales = await _db.Invoices.Where(i => i.InvoiceDate >= startMonth && i.Status != InvoiceStatus.Cancelled).SumAsync(i => (decimal?)i.TotalAmount) ?? 0m,
            MonthlyPurchases = await _db.PurchaseOrders.Where(p => p.OrderDate >= startMonth && p.Status != PurchaseOrderStatus.Cancelled).SumAsync(p => (decimal?)p.TotalAmount) ?? 0m,
            PendingLeaveRequests = await _db.LeaveRequests.CountAsync(l => l.Status == LeaveStatus.Pending),
            ActiveOpportunities = await _db.Opportunities.CountAsync(o => o.Stage != OpportunityStage.Won && o.Stage != OpportunityStage.Lost)
        });
    }

    [HttpGet("sales-by-customer")]
    public async Task<IActionResult> SalesByCustomer([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var q = _db.Invoices.Include(i => i.Customer).Where(i => i.Status != InvoiceStatus.Cancelled);
        if (from.HasValue) q = q.Where(i => i.InvoiceDate >= from);
        if (to.HasValue) q = q.Where(i => i.InvoiceDate <= to);
        var data = await q.GroupBy(i => new { i.CustomerId, i.Customer!.Name })
            .Select(g => new { g.Key.CustomerId, CustomerName = g.Key.Name, Total = g.Sum(x => x.TotalAmount), Count = g.Count() })
            .OrderByDescending(x => x.Total).ToListAsync();
        return Ok(data);
    }

    [HttpGet("inventory-valuation")]
    public async Task<IActionResult> InventoryValuation()
    {
        var data = await _db.Products
            .Where(p => p.IsActive)
            .Select(p => new
            {
                p.Id, p.Sku, p.Name, p.QuantityOnHand, p.CostPrice,
                Value = p.QuantityOnHand * p.CostPrice
            })
            .OrderByDescending(p => p.Value)
            .ToListAsync();
        return Ok(new { Total = data.Sum(d => d.Value), Items = data });
    }

    [HttpGet("accounts-receivable")]
    public async Task<IActionResult> AccountsReceivable()
    {
        var data = await _db.Invoices.Include(i => i.Customer)
            .Where(i => i.Status != InvoiceStatus.Paid && i.Status != InvoiceStatus.Cancelled)
            .Select(i => new
            {
                i.Id, i.InvoiceNumber, i.InvoiceDate, i.DueDate,
                CustomerName = i.Customer!.Name,
                i.TotalAmount, i.PaidAmount,
                BalanceDue = i.TotalAmount - i.PaidAmount,
                IsOverdue = i.DueDate < DateTime.UtcNow
            })
            .OrderBy(x => x.DueDate)
            .ToListAsync();
        return Ok(new { Total = data.Sum(d => d.BalanceDue), Items = data });
    }
}
