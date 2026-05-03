using ErpSystem.Domain.Common;
using ErpSystem.Domain.Enums;

namespace ErpSystem.Domain.Finance;

public class Account : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public AccountType Type { get; set; }
    public int? ParentAccountId { get; set; }
    public Account? ParentAccount { get; set; }
    public ICollection<Account> SubAccounts { get; set; } = new List<Account>();
    public decimal OpeningBalance { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }

    public ICollection<JournalEntryLine> JournalLines { get; set; } = new List<JournalEntryLine>();
}
