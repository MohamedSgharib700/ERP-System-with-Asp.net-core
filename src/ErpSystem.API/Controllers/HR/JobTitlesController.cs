using AutoMapper;
using ErpSystem.API.Controllers.Common;
using ErpSystem.Application.DTOs.HR;
using ErpSystem.Application.Interfaces;
using ErpSystem.Domain.HR;

namespace ErpSystem.API.Controllers.HR;

public class JobTitlesController : CrudControllerBase<JobTitle, JobTitleDto>
{
    public JobTitlesController(IApplicationDbContext db, IMapper mapper) : base(db, mapper) { }
}
