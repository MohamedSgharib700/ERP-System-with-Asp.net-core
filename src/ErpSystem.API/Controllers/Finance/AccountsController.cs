using AutoMapper;
using ErpSystem.API.Controllers.Common;
using ErpSystem.Application.DTOs.Finance;
using ErpSystem.Application.Interfaces;
using ErpSystem.Domain.Finance;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.API.Controllers.Finance;

public class AccountsController : CrudControllerBase<Account, AccountDto>
{
    public AccountsController(IApplicationDbContext db, IMapper mapper) : base(db, mapper) { }
    protected override IQueryable<Account> Query() => Db.Accounts.Include(a => a.ParentAccount).AsNoTracking();
}
