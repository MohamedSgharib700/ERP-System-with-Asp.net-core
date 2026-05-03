using AutoMapper;
using ErpSystem.API.Controllers.Common;
using ErpSystem.Application.DTOs.Inventory;
using ErpSystem.Application.Interfaces;
using ErpSystem.Domain.Inventory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.API.Controllers.Inventory;

public class ProductsController : CrudControllerBase<Product, ProductDto>
{
    public ProductsController(IApplicationDbContext db, IMapper mapper) : base(db, mapper) { }
    protected override IQueryable<Product> Query() =>
        Db.Products.Include(p => p.Category).Include(p => p.UnitOfMeasure).AsNoTracking();

    [HttpGet("low-stock")]
    public async Task<ActionResult<List<ProductDto>>> LowStock()
    {
        var items = await Db.Products
            .Include(p => p.Category)
            .Include(p => p.UnitOfMeasure)
            .Where(p => p.QuantityOnHand <= p.ReorderLevel && p.IsActive)
            .AsNoTracking()
            .ToListAsync();
        return Ok(Mapper.Map<List<ProductDto>>(items));
    }
}
