using AutoMapper;
using ErpSystem.API.Controllers.Common;
using ErpSystem.Application.DTOs.HR;
using ErpSystem.Application.Interfaces;
using ErpSystem.Domain.Enums;
using ErpSystem.Domain.HR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.API.Controllers.HR;

public class LeaveRequestsController : CrudControllerBase<LeaveRequest, LeaveRequestDto>
{
    public LeaveRequestsController(IApplicationDbContext db, IMapper mapper) : base(db, mapper) { }
    protected override IQueryable<LeaveRequest> Query() => Db.LeaveRequests.Include(l => l.Employee).AsNoTracking();

    [HttpPost("{id:int}/approve")]
    [Authorize(Roles = "Admin,Manager,HR")]
    public async Task<IActionResult> Approve(int id)
    {
        var lr = await Db.LeaveRequests.FindAsync(id);
        if (lr == null) return NotFound();
        lr.Status = LeaveStatus.Approved;
        lr.ApprovedAt = DateTime.UtcNow;
        await Db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("{id:int}/reject")]
    [Authorize(Roles = "Admin,Manager,HR")]
    public async Task<IActionResult> Reject(int id)
    {
        var lr = await Db.LeaveRequests.FindAsync(id);
        if (lr == null) return NotFound();
        lr.Status = LeaveStatus.Rejected;
        await Db.SaveChangesAsync();
        return Ok();
    }
}
