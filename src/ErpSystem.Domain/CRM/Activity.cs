using ErpSystem.Domain.Common;
using ErpSystem.Domain.Enums;

namespace ErpSystem.Domain.CRM;

public class Activity : BaseEntity
{
    public string Subject { get; set; } = string.Empty;
    public ActivityType Type { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public int? LeadId { get; set; }
    public int? OpportunityId { get; set; }
    public int? ContactId { get; set; }
    public string? AssignedTo { get; set; }
    public string? Description { get; set; }
}
