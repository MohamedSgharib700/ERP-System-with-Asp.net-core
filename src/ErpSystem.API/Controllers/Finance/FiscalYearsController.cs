using AutoMapper;
using ErpSystem.API.Controllers.Common;
using ErpSystem.Application.DTOs.Finance;
using ErpSystem.Application.Interfaces;
using ErpSystem.Domain.Finance;

namespace ErpSystem.API.Controllers.Finance;

public class FiscalYearsController : CrudControllerBase<FiscalYear, FiscalYearDto>
{
    public FiscalYearsController(IApplicationDbContext db, IMapper mapper) : base(db, mapper) { }
}
