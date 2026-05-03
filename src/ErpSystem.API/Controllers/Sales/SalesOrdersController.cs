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
[Authorize(Roles = "Admin,Sales,Manager")]
[Route("api/[controller]")]
public class SalesOrdersController : ControllerBase
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;
    public SalesOrdersController(IApplicationDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    [HttpGet]
    public async Task<ActionResult<PagedResult<SalesOrderDto>>> GetAll([FromQuery] PagedQuery q)
    {
        var query = _db.SalesOrders.Include(o => o.Customer).Include(o => o.Items).ThenInclude(i => i.Product).AsNoTracking();
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(o => o.Id).Skip((q.PageNumber - 1) * q.PageSize).Take(q.PageSize).ToListAsync();
        return Ok(new PagedResult<SalesOrderDto>
        {
            Items = _mapper.Map<List<SalesOrderDto>>(items),
            TotalCount = total, PageNumber = q.PageNumber, PageSize = q.PageSize
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SalesOrderDto>> GetById(int id)
    {
        var order = await _db.SalesOrders.Include(o => o.Customer).Include(o => o.Items).ThenInclude(i => i.Product)
            .AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        return order == null ? NotFound() : Ok(_mapper.Map<SalesOrderDto>(order));
    }

    [HttpPost]
    public async Task<ActionResult<SalesOrderDto>> Create([FromBody] SalesOrderDto dto)
    {
        if (!await _db.Customers.AnyAsync(c => c.Id == dto.CustomerId)) return BadRequest("Invalid customer");

        var order = new SalesOrder
        {
            OrderNumber = string.IsNullOrWhiteSpace(dto.OrderNumber) ? $"SO-{DateTime.UtcNow:yyyyMMddHHmmss}" : dto.OrderNumber,
            OrderDate = dto.OrderDate == default ? DateTime.UtcNow : dto.OrderDate,
            DeliveryDate = dto.DeliveryDate,
            CustomerId = dto.CustomerId,
            Status = SalesOrderStatus.Draft,
            Notes = dto.Notes,
            Items = dto.Items.Select(i => new SalesOrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Discount = i.Discount,
                TaxRate = i.TaxRate
            }).ToList()
        };
        order.SubTotal = order.Items.Sum(i => i.Quantity * i.UnitPrice);
        order.DiscountAmount = order.Items.Sum(i => i.Discount);
        order.TaxAmount = order.Items.Sum(i => (i.Quantity * i.UnitPrice - i.Discount) * i.TaxRate / 100);
        order.TotalAmount = order.SubTotal - order.DiscountAmount + order.TaxAmount;

        _db.SalesOrders.Add(order);
        await _db.SaveChangesAsync();
        return Ok(_mapper.Map<SalesOrderDto>(order));
    }

    [HttpPost("{id:int}/confirm")]
    public async Task<IActionResult> Confirm(int id)
    {
        var order = await _db.SalesOrders.FindAsync(id);
        if (order == null) return NotFound();
        order.Status = SalesOrderStatus.Confirmed;
        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var order = await _db.SalesOrders.FindAsync(id);
        if (order == null) return NotFound();
        order.Status = SalesOrderStatus.Cancelled;
        await _db.SaveChangesAsync();
        return Ok();
    }
}
