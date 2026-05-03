using AutoMapper;
using ErpSystem.API.Controllers.Common;
using ErpSystem.Application.DTOs.HR;
using ErpSystem.Application.Interfaces;
using ErpSystem.Domain.HR;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.API.Controllers.HR;

public class DepartmentsController : CrudControllerBase<Department, DepartmentDto>
{
    public DepartmentsController(IApplicationDbContext db, IMapper mapper) : base(db, mapper) { }
    protected override IQueryable<Department> Query() => Db.Departments.Include(d => d.ParentDepartment).AsNoTracking();
}
