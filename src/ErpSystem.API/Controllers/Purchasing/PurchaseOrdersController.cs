using AutoMapper;
using ErpSystem.Application.Common;
using ErpSystem.Application.DTOs.Purchasing;
using ErpSystem.Application.Interfaces;
using ErpSystem.Domain.Enums;
using ErpSystem.Domain.Inventory;
using ErpSystem.Domain.Purchasing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.API.Controllers.Purchasing;

[ApiController]
[Authorize(Roles = "Admin,Purchasing,Manager")]
[Route("api/[controller]")]
public class PurchaseOrdersController : ControllerBase
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;
    public PurchaseOrdersController(IApplicationDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    [HttpGet]
    public async Task<ActionResult<PagedResult<PurchaseOrderDto>>> GetAll([FromQuery] PagedQuery q)
    {
        var query = _db.PurchaseOrders.Include(o => o.Supplier).Include(o => o.Items).ThenInclude(i => i.Product).AsNoTracking();
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(o => o.Id).Skip((q.PageNumber - 1) * q.PageSize).Take(q.PageSize).ToListAsync();
        return Ok(new PagedResult<PurchaseOrderDto>
        {
            Items = _mapper.Map<List<PurchaseOrderDto>>(items),
            TotalCount = total, PageNumber = q.PageNumber, PageSize = q.PageSize
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PurchaseOrderDto>> GetById(int id)
    {
        var po = await _db.PurchaseOrders.Include(o => o.Supplier).Include(o => o.Items).ThenInclude(i => i.Product)
            .AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        return po == null ? NotFound() : Ok(_mapper.Map<PurchaseOrderDto>(po));
    }

    [HttpPost]
    public async Task<ActionResult<PurchaseOrderDto>> Create([FromBody] PurchaseOrderDto dto)
    {
        if (!await _db.Suppliers.AnyAsync(s => s.Id == dto.SupplierId)) return BadRequest("Invalid supplier");

        var po = new PurchaseOrder
        {
            OrderNumber = string.IsNullOrWhiteSpace(dto.OrderNumber) ? $"PO-{DateTime.UtcNow:yyyyMMddHHmmss}" : dto.OrderNumber,
            OrderDate = dto.OrderDate == default ? DateTime.UtcNow : dto.OrderDate,
            ExpectedDate = dto.ExpectedDate,
            SupplierId = dto.SupplierId,
            Status = PurchaseOrderStatus.Draft,
            Notes = dto.Notes,
            Items = dto.Items.Select(i => new PurchaseOrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitCost = i.UnitCost,
                Discount = i.Discount,
                TaxRate = i.TaxRate
            }).ToList()
        };
        po.SubTotal = po.Items.Sum(i => i.Quantity * i.UnitCost);
        po.DiscountAmount = po.Items.Sum(i => i.Discount);
        po.TaxAmount = po.Items.Sum(i => (i.Quantity * i.UnitCost - i.Discount) * i.TaxRate / 100);
        po.TotalAmount = po.SubTotal - po.DiscountAmount + po.TaxAmount;

        _db.PurchaseOrders.Add(po);
        await _db.SaveChangesAsync();
        return Ok(_mapper.Map<PurchaseOrderDto>(po));
    }

    [HttpPost("{id:int}/send")]
    public async Task<IActionResult> Send(int id)
    {
        var po = await _db.PurchaseOrders.FindAsync(id);
        if (po == null) return NotFound();
        po.Status = PurchaseOrderStatus.Sent;
        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("receive")]
    public async Task<IActionResult> Receive([FromBody] ReceivePurchaseOrderDto dto)
    {
        var po = await _db.PurchaseOrders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == dto.PurchaseOrderId);
        if (po == null) return NotFound();
        if (!await _db.Warehouses.AnyAsync(w => w.Id == dto.WarehouseId)) return BadRequest("Invalid warehouse");

        foreach (var line in dto.Items)
        {
            var item = po.Items.FirstOrDefault(i => i.Id == line.PurchaseOrderItemId);
            if (item == null) continue;
            item.ReceivedQuantity += line.ReceivedQuantity;

            var product = await _db.Products.FindAsync(item.ProductId);
            if (product != null)
            {
                product.QuantityOnHand += line.ReceivedQuantity;
                _db.StockMovements.Add(new StockMovement
                {
                    ProductId = product.Id,
                    WarehouseId = dto.WarehouseId,
                    MovementType = StockMovementType.In,
                    Quantity = line.ReceivedQuantity,
                    UnitCost = item.UnitCost,
                    MovementDate = DateTime.UtcNow,
                    Reference = po.OrderNumber,
                    Notes = "Received from PO"
                });
            }
        }

        if (po.Items.All(i => i.ReceivedQuantity >= i.Quantity)) po.Status = PurchaseOrderStatus.Received;
        await _db.SaveChangesAsync();
        return Ok();
    }
}
