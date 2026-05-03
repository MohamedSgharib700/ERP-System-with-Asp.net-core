using AutoMapper;
using ErpSystem.API.Controllers.Common;
using ErpSystem.Application.DTOs.Inventory;
using ErpSystem.Application.Interfaces;
using ErpSystem.Domain.Inventory;

namespace ErpSystem.API.Controllers.Inventory;

public class UnitsOfMeasureController : CrudControllerBase<UnitOfMeasure, UnitOfMeasureDto>
{
    public UnitsOfMeasureController(IApplicationDbContext db, IMapper mapper) : base(db, mapper) { }
}
