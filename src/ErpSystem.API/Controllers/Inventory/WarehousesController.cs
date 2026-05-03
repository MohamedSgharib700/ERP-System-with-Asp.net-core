using AutoMapper;
using ErpSystem.API.Controllers.Common;
using ErpSystem.Application.DTOs.Inventory;
using ErpSystem.Application.Interfaces;
using ErpSystem.Domain.Inventory;

namespace ErpSystem.API.Controllers.Inventory;

public class WarehousesController : CrudControllerBase<Warehouse, WarehouseDto>
{
    public WarehousesController(IApplicationDbContext db, IMapper mapper) : base(db, mapper) { }
}
