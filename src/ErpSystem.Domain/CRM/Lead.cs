using ErpSystem.Domain.Common;
using ErpSystem.Domain.Enums;

namespace ErpSystem.Domain.CRM;

public class Lead : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Company { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Source { get; set; }
    public LeadStatus Status { get; set; } = LeadStatus.New;
    public decimal EstimatedValue { get; set; }
    public string? AssignedTo { get; set; }
    public string? Notes { get; set; }
}
