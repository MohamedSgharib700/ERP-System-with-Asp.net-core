using AutoMapper;
using ErpSystem.API.Controllers.Common;
using ErpSystem.Application.DTOs.HR;
using ErpSystem.Application.Interfaces;
using ErpSystem.Domain.HR;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.API.Controllers.HR;

public class EmployeesController : CrudControllerBase<Employee, EmployeeDto>
{
    public EmployeesController(IApplicationDbContext db, IMapper mapper) : base(db, mapper) { }

    protected override IQueryable<Employee> Query() =>
        Db.Employees
            .Include(e => e.Department)
            .Include(e => e.JobTitle)
            .Include(e => e.Manager)
            .AsNoTracking();
}
