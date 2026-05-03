using AutoMapper;
using ErpSystem.API.Controllers.Common;
using ErpSystem.Application.DTOs.Sales;
using ErpSystem.Application.Interfaces;
using ErpSystem.Domain.Sales;

namespace ErpSystem.API.Controllers.Sales;

public class CustomersController : CrudControllerBase<Customer, CustomerDto>
{
    public CustomersController(IApplicationDbContext db, IMapper mapper) : base(db, mapper) { }
}
