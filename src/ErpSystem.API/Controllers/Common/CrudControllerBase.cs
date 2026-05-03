using AutoMapper;
using ErpSystem.Application.Common;
using ErpSystem.Application.Interfaces;
using ErpSystem.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.API.Controllers.Common;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public abstract class CrudControllerBase<TEntity, TDto> : ControllerBase
    where TEntity : BaseEntity, new()
    where TDto : class
{
    protected readonly IApplicationDbContext Db;
    protected readonly IMapper Mapper;

    protected CrudControllerBase(IApplicationDbContext db, IMapper mapper)
    {
        Db = db;
        Mapper = mapper;
    }

    protected virtual IQueryable<TEntity> Query() => Db.Set<TEntity>().AsNoTracking();

    [HttpGet]
    public virtual async Task<ActionResult<PagedResult<TDto>>> GetAll([FromQuery] PagedQuery q)
    {
        var query = Query();
        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(e => e.Id)
            .Skip((q.PageNumber - 1) * q.PageSize)
            .Take(q.PageSize)
            .ToListAsync();
        var dtos = Mapper.Map<List<TDto>>(items);
        return Ok(new PagedResult<TDto>
        {
            Items = dtos,
            TotalCount = total,
            PageNumber = q.PageNumber,
            PageSize = q.PageSize
        });
    }

    [HttpGet("{id:int}")]
    public virtual async Task<ActionResult<TDto>> GetById(int id)
    {
        var entity = await Query().FirstOrDefaultAsync(e => e.Id == id);
        return entity == null ? NotFound() : Ok(Mapper.Map<TDto>(entity));
    }

    [HttpPost]
    public virtual async Task<ActionResult<TDto>> Create([FromBody] TDto dto)
    {
        var entity = Mapper.Map<TEntity>(dto);
        entity.Id = 0;
        Db.Set<TEntity>().Add(entity);
        await Db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, Mapper.Map<TDto>(entity));
    }

    [HttpPut("{id:int}")]
    public virtual async Task<ActionResult<TDto>> Update(int id, [FromBody] TDto dto)
    {
        var entity = await Db.Set<TEntity>().FindAsync(id);
        if (entity == null) return NotFound();
        Mapper.Map(dto, entity);
        entity.Id = id;
        await Db.SaveChangesAsync();
        return Ok(Mapper.Map<TDto>(entity));
    }

    [HttpDelete("{id:int}")]
    public virtual async Task<IActionResult> Delete(int id)
    {
        var entity = await Db.Set<TEntity>().FindAsync(id);
        if (entity == null) return NotFound();
        entity.IsDeleted = true;
        await Db.SaveChangesAsync();
        return NoContent();
    }
}
