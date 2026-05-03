using ErpSystem.Domain.Common;
using ErpSystem.Domain.Enums;

namespace ErpSystem.Domain.HR;

public class Attendance : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan? CheckIn { get; set; }
    public TimeSpan? CheckOut { get; set; }
    public AttendanceStatus Status { get; set; }
    public string? Notes { get; set; }
}
