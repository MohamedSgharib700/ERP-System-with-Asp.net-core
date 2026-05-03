using AutoMapper;
using ErpSystem.Application.Common;
using ErpSystem.Application.DTOs.Finance;
using ErpSystem.Application.Interfaces;
using ErpSystem.Domain.Enums;
using ErpSystem.Domain.Finance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.API.Controllers.Finance;

[ApiController]
[Authorize(Roles = "Admin,Accountant")]
[Route("api/[controller]")]
public class JournalEntriesController : ControllerBase
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;
    public JournalEntriesController(IApplicationDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    [HttpGet]
    public async Task<ActionResult<PagedResult<JournalEntryDto>>> GetAll([FromQuery] PagedQuery q)
    {
        var query = _db.JournalEntries.Include(j => j.Lines).ThenInclude(l => l.Account).AsNoTracking();
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(j => j.Id).Skip((q.PageNumber - 1) * q.PageSize).Take(q.PageSize).ToListAsync();
        return Ok(new PagedResult<JournalEntryDto>
        {
            Items = _mapper.Map<List<JournalEntryDto>>(items),
            TotalCount = total, PageNumber = q.PageNumber, PageSize = q.PageSize
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<JournalEntryDto>> GetById(int id)
    {
        var je = await _db.JournalEntries.Include(j => j.Lines).ThenInclude(l => l.Account)
            .AsNoTracking().FirstOrDefaultAsync(j => j.Id == id);
        return je == null ? NotFound() : Ok(_mapper.Map<JournalEntryDto>(je));
    }

    [HttpPost]
    public async Task<ActionResult<JournalEntryDto>> Create([FromBody] JournalEntryDto dto)
    {
        if (dto.Lines.Sum(l => l.Debit) != dto.Lines.Sum(l => l.Credit))
            return BadRequest("Journal entry is not balanced (Debit must equal Credit).");

        var fy = await _db.FiscalYears.FindAsync(dto.FiscalYearId);
        if (fy == null) return BadRequest("Invalid FiscalYearId");

        var entry = new JournalEntry
        {
            EntryNumber = string.IsNullOrWhiteSpace(dto.EntryNumber) ? $"JE-{DateTime.UtcNow:yyyyMMddHHmmss}" : dto.EntryNumber,
            Date = dto.Date == default ? DateTime.UtcNow : dto.Date,
            Reference = dto.Reference,
            Description = dto.Description,
            Status = JournalStatus.Draft,
            FiscalYearId = dto.FiscalYearId,
            Lines = dto.Lines.Select(l => new JournalEntryLine
            {
                AccountId = l.AccountId,
                Debit = l.Debit,
                Credit = l.Credit,
                Description = l.Description
            }).ToList() 
        };
        _db.JournalEntries.Add(entry);
        await _db.SaveChangesAsync();
        return Ok(_mapper.Map<JournalEntryDto>(entry));
    }

    [HttpPost("{id:int}/post")]
    public async Task<IActionResult> PostEntry(int id)
    {
        var je = await _db.JournalEntries.FindAsync(id);
        if (je == null) return NotFound();
        if (je.Status == JournalStatus.Posted) return BadRequest("Already posted");
        je.Status = JournalStatus.Posted;
        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var je = await _db.JournalEntries.FindAsync(id);
        if (je == null) return NotFound();
        if (je.Status == JournalStatus.Posted) return BadRequest("Cannot delete a posted journal");
        je.IsDeleted = true;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("trial-balance")]
    public async Task<ActionResult<List<TrialBalanceRowDto>>> TrialBalance([FromQuery] int? fiscalYearId)
    {
        var lines = _db.JournalEntryLines
            .Include(l => l.Account)
            .Include(l => l.JournalEntry)
            .Where(l => l.JournalEntry!.Status == JournalStatus.Posted);
        if (fiscalYearId.HasValue) lines = lines.Where(l => l.JournalEntry!.FiscalYearId == fiscalYearId.Value);

        var grouped = await lines
            .GroupBy(l => new { l.Account!.Code, l.Account.Name, l.Account.Type, l.Account.OpeningBalance })
            .Select(g => new TrialBalanceRowDto
            {
                AccountCode = g.Key.Code,
                AccountName = g.Key.Name,
                Type = g.Key.Type,
                Debit = g.Sum(x => x.Debit),
                Credit = g.Sum(x => x.Credit),
                Balance = g.Key.OpeningBalance + g.Sum(x => x.Debit) - g.Sum(x => x.Credit)
            })
            .OrderBy(r => r.AccountCode)
            .ToListAsync();
        return Ok(grouped);
    }
}
