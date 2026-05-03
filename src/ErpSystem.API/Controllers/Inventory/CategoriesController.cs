using AutoMapper;
using ErpSystem.API.Controllers.Common;
using ErpSystem.Application.DTOs.Inventory;
using ErpSystem.Application.Interfaces;
using ErpSystem.Domain.Inventory;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.API.Controllers.Inventory;

public class CategoriesController : CrudControllerBase<Category, CategoryDto>
{
    public CategoriesController(IApplicationDbContext db, IMapper mapper) : base(db, mapper) { }
    protected override IQueryable<Category> Query() => Db.Categories.Include(c => c.ParentCategory).AsNoTracking();
}
