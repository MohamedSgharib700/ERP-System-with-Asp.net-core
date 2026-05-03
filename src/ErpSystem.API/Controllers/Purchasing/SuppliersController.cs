using AutoMapper;
using ErpSystem.API.Controllers.Common;
using ErpSystem.Application.DTOs.Purchasing;
using ErpSystem.Application.Interfaces;
using ErpSystem.Domain.Purchasing;

namespace ErpSystem.API.Controllers.Purchasing;

public class SuppliersController : CrudControllerBase<Supplier, SupplierDto>
{
    public SuppliersController(IApplicationDbContext db, IMapper mapper) : base(db, mapper) { }
}
