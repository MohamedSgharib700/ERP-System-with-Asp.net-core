using AutoMapper;
using ErpSystem.API.Controllers.Common;
using ErpSystem.Application.DTOs.HR;
using ErpSystem.Application.Interfaces;
using ErpSystem.Domain.HR;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.API.Controllers.HR;

public class AttendancesController : CrudControllerBase<Attendance, AttendanceDto>
{
    public AttendancesController(IApplicationDbContext db, IMapper mapper) : base(db, mapper) { }
    protected override IQueryable<Attendance> Query() => Db.Attendances.Include(a => a.Employee).AsNoTracking();
}
