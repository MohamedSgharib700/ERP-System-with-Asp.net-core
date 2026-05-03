using ErpSystem.Domain.Common;
using ErpSystem.Domain.Enums;

namespace ErpSystem.Domain.HR;

public class LeaveRequest : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public LeaveType LeaveType { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int Days => (ToDate - FromDate).Days + 1;
    public string? Reason { get; set; }
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
}
