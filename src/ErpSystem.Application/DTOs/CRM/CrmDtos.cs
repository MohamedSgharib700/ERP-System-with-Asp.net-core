using ErpSystem.Domain.Enums;

namespace ErpSystem.Application.DTOs.CRM;

public class LeadDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Company { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Source { get; set; }
    public LeadStatus Status { get; set; }
    public decimal EstimatedValue { get; set; }
    public string? AssignedTo { get; set; }
    public string? Notes { get; set; }
}

public class ContactDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? JobTitle { get; set; }
    public string? Company { get; set; }
    public string? Address { get; set; }
    public int? CustomerId { get; set; }
}

public class OpportunityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public int? LeadId { get; set; }
    public OpportunityStage Stage { get; set; }
    public decimal Amount { get; set; }
    public decimal Probability { get; set; }
    public DateTime? ExpectedCloseDate { get; set; }
    public string? AssignedTo { get; set; }
    public string? Description { get; set; }
}

public class ActivityDto
{
    public int Id { get; set; }
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
