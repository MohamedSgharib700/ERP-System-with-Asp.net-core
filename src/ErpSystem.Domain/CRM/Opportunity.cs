using ErpSystem.Domain.Common;
using ErpSystem.Domain.Enums;

namespace ErpSystem.Domain.CRM;

public class Opportunity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public int? LeadId { get; set; }
    public Lead? Lead { get; set; }
    public OpportunityStage Stage { get; set; } = OpportunityStage.Prospecting;
    public decimal Amount { get; set; }
    public decimal Probability { get; set; }
    public DateTime? ExpectedCloseDate { get; set; }
    public string? AssignedTo { get; set; }
    public string? Description { get; set; }
}
