using ErpSystem.Domain.Common;

namespace ErpSystem.Domain.CRM;

public class Contact : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? JobTitle { get; set; }
    public string? Company { get; set; }
    public string? Address { get; set; }
    public int? CustomerId { get; set; }
}
