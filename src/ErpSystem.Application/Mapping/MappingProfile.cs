using AutoMapper;
using ErpSystem.Application.DTOs.CRM;
using ErpSystem.Application.DTOs.Finance;
using ErpSystem.Application.DTOs.HR;
using ErpSystem.Application.DTOs.Inventory;
using ErpSystem.Application.DTOs.Purchasing;
using ErpSystem.Application.DTOs.Sales;
using ErpSystem.Domain.CRM;
using ErpSystem.Domain.Finance;
using ErpSystem.Domain.HR;
using ErpSystem.Domain.Inventory;
using ErpSystem.Domain.Purchasing;
using ErpSystem.Domain.Sales;

namespace ErpSystem.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // HR
        CreateMap<Department, DepartmentDto>()
            .ForMember(d => d.ParentDepartmentName, o => o.MapFrom(s => s.ParentDepartment != null ? s.ParentDepartment.Name : null));
        CreateMap<DepartmentDto, Department>();

        CreateMap<JobTitle, JobTitleDto>().ReverseMap();

        CreateMap<Employee, EmployeeDto>()
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.FirstName + " " + s.LastName))
            .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Department != null ? s.Department.Name : null))
            .ForMember(d => d.JobTitleName, o => o.MapFrom(s => s.JobTitle != null ? s.JobTitle.Title : null))
            .ForMember(d => d.ManagerName, o => o.MapFrom(s => s.Manager != null ? s.Manager.FirstName + " " + s.Manager.LastName : null));
        CreateMap<EmployeeDto, Employee>();

        CreateMap<Attendance, AttendanceDto>()
            .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Employee != null ? s.Employee.FirstName + " " + s.Employee.LastName : null));
        CreateMap<AttendanceDto, Attendance>();

        CreateMap<LeaveRequest, LeaveRequestDto>()
            .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Employee != null ? s.Employee.FirstName + " " + s.Employee.LastName : null))
            .ForMember(d => d.Days, o => o.MapFrom(s => (s.ToDate - s.FromDate).Days + 1));
        CreateMap<LeaveRequestDto, LeaveRequest>();

        // Finance
        CreateMap<Account, AccountDto>()
            .ForMember(d => d.ParentAccountName, o => o.MapFrom(s => s.ParentAccount != null ? s.ParentAccount.Name : null));
        CreateMap<AccountDto, Account>();

        CreateMap<FiscalYear, FiscalYearDto>().ReverseMap();

        CreateMap<JournalEntry, JournalEntryDto>()
            .ForMember(d => d.TotalDebit, o => o.MapFrom(s => s.Lines.Sum(l => l.Debit)))
            .ForMember(d => d.TotalCredit, o => o.MapFrom(s => s.Lines.Sum(l => l.Credit)));
        CreateMap<JournalEntryDto, JournalEntry>();

        CreateMap<JournalEntryLine, JournalEntryLineDto>()
            .ForMember(d => d.AccountName, o => o.MapFrom(s => s.Account != null ? s.Account.Name : null));
        CreateMap<JournalEntryLineDto, JournalEntryLine>();

        // Inventory
        CreateMap<Category, CategoryDto>()
            .ForMember(d => d.ParentCategoryName, o => o.MapFrom(s => s.ParentCategory != null ? s.ParentCategory.Name : null));
        CreateMap<CategoryDto, Category>();

        CreateMap<UnitOfMeasure, UnitOfMeasureDto>().ReverseMap();
        CreateMap<Warehouse, WarehouseDto>().ReverseMap();

        CreateMap<Product, ProductDto>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category != null ? s.Category.Name : null))
            .ForMember(d => d.UnitOfMeasureName, o => o.MapFrom(s => s.UnitOfMeasure != null ? s.UnitOfMeasure.Name : null));
        CreateMap<ProductDto, Product>();

        CreateMap<StockMovement, StockMovementDto>()
            .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product != null ? s.Product.Name : null))
            .ForMember(d => d.WarehouseName, o => o.MapFrom(s => s.Warehouse != null ? s.Warehouse.Name : null));
        CreateMap<StockMovementDto, StockMovement>();

        // Sales
        CreateMap<Customer, CustomerDto>().ReverseMap();

        CreateMap<SalesOrder, SalesOrderDto>()
            .ForMember(d => d.CustomerName, o => o.MapFrom(s => s.Customer != null ? s.Customer.Name : null));
        CreateMap<SalesOrderDto, SalesOrder>();

        CreateMap<SalesOrderItem, SalesOrderItemDto>()
            .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product != null ? s.Product.Name : null))
            .ForMember(d => d.LineTotal, o => o.MapFrom(s => (s.Quantity * s.UnitPrice) - s.Discount + ((s.Quantity * s.UnitPrice - s.Discount) * s.TaxRate / 100)));
        CreateMap<SalesOrderItemDto, SalesOrderItem>();

        CreateMap<Invoice, InvoiceDto>()
            .ForMember(d => d.CustomerName, o => o.MapFrom(s => s.Customer != null ? s.Customer.Name : null))
            .ForMember(d => d.BalanceDue, o => o.MapFrom(s => s.TotalAmount - s.PaidAmount));
        CreateMap<InvoiceDto, Invoice>();

        CreateMap<InvoiceItem, InvoiceItemDto>()
            .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product != null ? s.Product.Name : null))
            .ForMember(d => d.LineTotal, o => o.MapFrom(s => (s.Quantity * s.UnitPrice) - s.Discount + ((s.Quantity * s.UnitPrice - s.Discount) * s.TaxRate / 100)));
        CreateMap<InvoiceItemDto, InvoiceItem>();

        // Purchasing
        CreateMap<Supplier, SupplierDto>().ReverseMap();

        CreateMap<PurchaseOrder, PurchaseOrderDto>()
            .ForMember(d => d.SupplierName, o => o.MapFrom(s => s.Supplier != null ? s.Supplier.Name : null));
        CreateMap<PurchaseOrderDto, PurchaseOrder>();

        CreateMap<PurchaseOrderItem, PurchaseOrderItemDto>()
            .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product != null ? s.Product.Name : null))
            .ForMember(d => d.LineTotal, o => o.MapFrom(s => (s.Quantity * s.UnitCost) - s.Discount + ((s.Quantity * s.UnitCost - s.Discount) * s.TaxRate / 100)));
        CreateMap<PurchaseOrderItemDto, PurchaseOrderItem>();

        // CRM
        CreateMap<Lead, LeadDto>().ReverseMap();
        CreateMap<Contact, ContactDto>().ReverseMap();
        CreateMap<Opportunity, OpportunityDto>().ReverseMap();
        CreateMap<Activity, ActivityDto>().ReverseMap();
    }
}
