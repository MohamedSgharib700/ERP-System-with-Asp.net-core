using AutoMapper;
using ErpSystem.Application.Common;
using ErpSystem.Application.DTOs.Inventory;
using ErpSystem.Application.Interfaces;
using ErpSystem.Domain.Enums;
using ErpSystem.Domain.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.API.Controllers.Inventory;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class StockMovementsController : ControllerBase
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;
    public StockMovementsController(IApplicationDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    [HttpGet]
    public async Task<ActionResult<PagedResult<StockMovementDto>>> GetAll([FromQuery] PagedQuery q)
    {
        var query = _db.StockMovements.Include(s => s.Product).Include(s => s.Warehouse).AsNoTracking();
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(s => s.Id).Skip((q.PageNumber - 1) * q.PageSize).Take(q.PageSize).ToListAsync();
        return Ok(new PagedResult<StockMovementDto>
        {
            Items = _mapper.Map<List<StockMovementDto>>(items),
            TotalCount = total, PageNumber = q.PageNumber, PageSize = q.PageSize
        });
    }

    [HttpPost]
    public async Task<ActionResult<StockMovementDto>> Create([FromBody] StockMovementDto dto)
    {
        var product = await _db.Products.FindAsync(dto.ProductId);
        if (product == null) return BadRequest("Invalid product");
        var warehouse = await _db.Warehouses.FindAsync(dto.WarehouseId);
        if (warehouse == null) return BadRequest("Invalid warehouse");

        var movement = _mapper.Map<StockMovement>(dto);
        movement.Id = 0;
        if (movement.MovementDate == default) movement.MovementDate = DateTime.UtcNow;

        switch (movement.MovementType)
        {
            case StockMovementType.In:
                product.QuantityOnHand += movement.Quantity;
                break;
            case StockMovementType.Out:
                if (product.QuantityOnHand < movement.Quantity)
                    return BadRequest("Insufficient stock");
                product.QuantityOnHand -= movement.Quantity;
                break;
            case StockMovementType.Adjustment:
                product.QuantityOnHand += movement.Quantity;
                break;
        }

        _db.StockMovements.Add(movement);
        await _db.SaveChangesAsync();
        return Ok(_mapper.Map<StockMovementDto>(movement));
    }
}
