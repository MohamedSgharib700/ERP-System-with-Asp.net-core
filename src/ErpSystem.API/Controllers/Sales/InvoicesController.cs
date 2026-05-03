using AutoMapper;
using ErpSystem.Application.Common;
using ErpSystem.Application.DTOs.Sales;
using ErpSystem.Application.Interfaces;
using ErpSystem.Domain.Enums;
using ErpSystem.Domain.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.API.Controllers.Sales;

[ApiController]
[Authorize(Roles = "Admin,Sales,Accountant,Manager")]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;
    public InvoicesController(IApplicationDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    [HttpGet]
    public async Task<ActionResult<PagedResult<InvoiceDto>>> GetAll([FromQuery] PagedQuery q)
    {
        var query = _db.Invoices.Include(i => i.Customer).Include(i => i.Items).ThenInclude(it => it.Product).AsNoTracking();
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(i => i.Id).Skip((q.PageNumber - 1) * q.PageSize).Take(q.PageSize).ToListAsync();
        return Ok(new PagedResult<InvoiceDto>
        {
            Items = _mapper.Map<List<InvoiceDto>>(items),
            TotalCount = total, PageNumber = q.PageNumber, PageSize = q.PageSize
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<InvoiceDto>> GetById(int id)
    {
        var inv = await _db.Invoices.Include(i => i.Customer).Include(i => i.Items).ThenInclude(it => it.Product)
            .AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        return inv == null ? NotFound() : Ok(_mapper.Map<InvoiceDto>(inv));
    }

    [HttpPost]
    public async Task<ActionResult<InvoiceDto>> Create([FromBody] InvoiceDto dto)
    {
        if (!await _db.Customers.AnyAsync(c => c.Id == dto.CustomerId)) return BadRequest("Invalid customer");

        var invoice = new Invoice
        {
            InvoiceNumber = string.IsNullOrWhiteSpace(dto.InvoiceNumber) ? $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}" : dto.InvoiceNumber,
            InvoiceDate = dto.InvoiceDate == default ? DateTime.UtcNow : dto.InvoiceDate,
            DueDate = dto.DueDate == default ? DateTime.UtcNow.AddDays(30) : dto.DueDate,
            CustomerId = dto.CustomerId,
            SalesOrderId = dto.SalesOrderId,
            Status = InvoiceStatus.Issued,
            Notes = dto.Notes,
            Items = dto.Items.Select(i => new InvoiceItem
            {
                ProductId = i.ProductId,
                Description = i.Description,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Discount = i.Discount,
                TaxRate = i.TaxRate
            }).ToList()
        };
        invoice.SubTotal = invoice.Items.Sum(i => i.Quantity * i.UnitPrice);
        invoice.DiscountAmount = invoice.Items.Sum(i => i.Discount);
        invoice.TaxAmount = invoice.Items.Sum(i => (i.Quantity * i.UnitPrice - i.Discount) * i.TaxRate / 100);
        invoice.TotalAmount = invoice.SubTotal - invoice.DiscountAmount + invoice.TaxAmount;

        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();
        return Ok(_mapper.Map<InvoiceDto>(invoice));
    }

    [HttpPost("payment")]
    public async Task<IActionResult> RecordPayment([FromBody] RecordPaymentDto dto)
    {
        var inv = await _db.Invoices.FindAsync(dto.InvoiceId);
        if (inv == null) return NotFound();
        if (dto.Amount <= 0) return BadRequest("Amount must be positive");
        inv.PaidAmount += dto.Amount;
        if (inv.PaidAmount >= inv.TotalAmount) inv.Status = InvoiceStatus.Paid;
        else if (inv.PaidAmount > 0) inv.Status = InvoiceStatus.PartiallyPaid;
        await _db.SaveChangesAsync();
        return Ok(_mapper.Map<InvoiceDto>(inv));
    }
}
