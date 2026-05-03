using Microsoft.AspNetCore.Identity;

namespace ErpSystem.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}

public class ApplicationRole : IdentityRole
{
    public string? Description { get; set; }
}
