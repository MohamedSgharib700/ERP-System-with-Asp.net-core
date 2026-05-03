using AutoMapper;
using ErpSystem.API.Controllers.Common;
using ErpSystem.Application.DTOs.CRM;
using ErpSystem.Application.Interfaces;
using ErpSystem.Domain.CRM;

namespace ErpSystem.API.Controllers.CRM;

public class LeadsController : CrudControllerBase<Lead, LeadDto>
{
    public LeadsController(IApplicationDbContext db, IMapper mapper) : base(db, mapper) { }
}

public class ContactsController : CrudControllerBase<Contact, ContactDto>
{
    public ContactsController(IApplicationDbContext db, IMapper mapper) : base(db, mapper) { }
}

public class OpportunitiesController : CrudControllerBase<Opportunity, OpportunityDto>
{
    public OpportunitiesController(IApplicationDbContext db, IMapper mapper) : base(db, mapper) { }
}

public class ActivitiesController : CrudControllerBase<Activity, ActivityDto>
{
    public ActivitiesController(IApplicationDbContext db, IMapper mapper) : base(db, mapper) { }
}
