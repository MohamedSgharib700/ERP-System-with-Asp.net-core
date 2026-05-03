using ErpSystem.Domain.Common;

namespace ErpSystem.Domain.HR;

public class JobTitle : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal MinSalary { get; set; }
    public decimal MaxSalary { get; set; }
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
