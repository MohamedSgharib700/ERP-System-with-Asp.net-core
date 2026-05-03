using ErpSystem.Application.Interfaces;
using ErpSystem.Domain.Common;
using ErpSystem.Domain.CRM;
using ErpSystem.Domain.Finance;
using ErpSystem.Domain.HR;
using ErpSystem.Domain.Inventory;
using ErpSystem.Domain.Purchasing;
using ErpSystem.Domain.Sales;
using ErpSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>, IApplicationDbContext
{
    private readonly ICurrentUserService? _currentUser;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService? currentUser = null)
        : base(options)
    {
        _currentUser = currentUser;
    }

    // HR
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<JobTitle> JobTitles => Set<JobTitle>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();

    // Finance
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<FiscalYear> FiscalYears => Set<FiscalYear>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<JournalEntryLine> JournalEntryLines => Set<JournalEntryLine>();

    // Inventory
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<UnitOfMeasure> UnitsOfMeasure => Set<UnitOfMeasure>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();

    // Sales
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
    public DbSet<SalesOrderItem> SalesOrderItems => Set<SalesOrderItem>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();

    // Purchasing
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();

    // CRM
    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<Opportunity> Opportunities => Set<Opportunity>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Activity> Activities => Set<Activity>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Identity table names
        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<ApplicationRole>().ToTable("Roles");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>().ToTable("UserRoles");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>().ToTable("UserLogins");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>().ToTable("RoleClaims");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>().ToTable("UserTokens");

        // HR configuration
        builder.Entity<Employee>(b =>
        {
            b.HasIndex(e => e.EmployeeNumber).IsUnique();
            b.Property(e => e.Salary).HasPrecision(18, 2);
            b.Ignore(e => e.FullName);
            b.HasOne(e => e.Department).WithMany(d => d.Employees).HasForeignKey(e => e.DepartmentId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(e => e.JobTitle).WithMany(j => j.Employees).HasForeignKey(e => e.JobTitleId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(e => e.Manager).WithMany().HasForeignKey(e => e.ManagerId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Department>().HasOne(d => d.ParentDepartment).WithMany(d => d.SubDepartments)
            .HasForeignKey(d => d.ParentDepartmentId).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<JobTitle>(b =>
        {
            b.Property(e => e.MinSalary).HasPrecision(18, 2);
            b.Property(e => e.MaxSalary).HasPrecision(18, 2);
        });

        builder.Entity<LeaveRequest>().Ignore(l => l.Days);

        // Finance
        builder.Entity<Account>(b =>
        {
            b.HasIndex(a => a.Code).IsUnique();
            b.Property(a => a.OpeningBalance).HasPrecision(18, 2);
            b.HasOne(a => a.ParentAccount).WithMany(a => a.SubAccounts)
                .HasForeignKey(a => a.ParentAccountId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<JournalEntry>(b =>
        {
            b.HasIndex(j => j.EntryNumber).IsUnique();
            b.Ignore(j => j.TotalDebit);
            b.Ignore(j => j.TotalCredit);
        });

        builder.Entity<JournalEntryLine>(b =>
        {
            b.Property(l => l.Debit).HasPrecision(18, 2);
            b.Property(l => l.Credit).HasPrecision(18, 2);
            b.HasOne(l => l.Account).WithMany(a => a.JournalLines).HasForeignKey(l => l.AccountId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(l => l.JournalEntry).WithMany(j => j.Lines).HasForeignKey(l => l.JournalEntryId).OnDelete(DeleteBehavior.Cascade);
        });

        // Inventory
        builder.Entity<Category>().HasOne(c => c.ParentCategory).WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Warehouse>().HasIndex(w => w.Code).IsUnique();

        builder.Entity<Product>(b =>
        {
            b.HasIndex(p => p.Sku).IsUnique();
            b.Property(p => p.CostPrice).HasPrecision(18, 2);
            b.Property(p => p.SellingPrice).HasPrecision(18, 2);
            b.Property(p => p.QuantityOnHand).HasPrecision(18, 2);
            b.Property(p => p.ReorderLevel).HasPrecision(18, 2);
            b.HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(p => p.UnitOfMeasure).WithMany(u => u.Products).HasForeignKey(p => p.UnitOfMeasureId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<StockMovement>(b =>
        {
            b.Property(s => s.Quantity).HasPrecision(18, 2);
            b.Property(s => s.UnitCost).HasPrecision(18, 2);
            b.HasOne(s => s.Product).WithMany(p => p.StockMovements).HasForeignKey(s => s.ProductId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(s => s.Warehouse).WithMany(w => w.StockMovements).HasForeignKey(s => s.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        });

        // Sales
        builder.Entity<Customer>(b =>
        {
            b.HasIndex(c => c.Code).IsUnique();
            b.Property(c => c.CreditLimit).HasPrecision(18, 2);
            b.Property(c => c.CurrentBalance).HasPrecision(18, 2);
        });

        builder.Entity<SalesOrder>(b =>
        {
            b.HasIndex(o => o.OrderNumber).IsUnique();
            b.Property(o => o.SubTotal).HasPrecision(18, 2);
            b.Property(o => o.TaxAmount).HasPrecision(18, 2);
            b.Property(o => o.DiscountAmount).HasPrecision(18, 2);
            b.Property(o => o.TotalAmount).HasPrecision(18, 2);
        });

        builder.Entity<SalesOrderItem>(b =>
        {
            b.Property(i => i.Quantity).HasPrecision(18, 2);
            b.Property(i => i.UnitPrice).HasPrecision(18, 2);
            b.Property(i => i.Discount).HasPrecision(18, 2);
            b.Property(i => i.TaxRate).HasPrecision(18, 2);
            b.Ignore(i => i.LineTotal);
            b.HasOne(i => i.SalesOrder).WithMany(o => o.Items).HasForeignKey(i => i.SalesOrderId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(i => i.Product).WithMany().HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Invoice>(b =>
        {
            b.HasIndex(i => i.InvoiceNumber).IsUnique();
            b.Property(i => i.SubTotal).HasPrecision(18, 2);
            b.Property(i => i.TaxAmount).HasPrecision(18, 2);
            b.Property(i => i.DiscountAmount).HasPrecision(18, 2);
            b.Property(i => i.TotalAmount).HasPrecision(18, 2);
            b.Property(i => i.PaidAmount).HasPrecision(18, 2);
            b.Ignore(i => i.BalanceDue);
            b.HasOne(i => i.SalesOrder).WithMany().HasForeignKey(i => i.SalesOrderId).OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<InvoiceItem>(b =>
        {
            b.Property(i => i.Quantity).HasPrecision(18, 2);
            b.Property(i => i.UnitPrice).HasPrecision(18, 2);
            b.Property(i => i.Discount).HasPrecision(18, 2);
            b.Property(i => i.TaxRate).HasPrecision(18, 2);
            b.Ignore(i => i.LineTotal);
            b.HasOne(i => i.Invoice).WithMany(inv => inv.Items).HasForeignKey(i => i.InvoiceId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(i => i.Product).WithMany().HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Restrict);
        });

        // Purchasing
        builder.Entity<Supplier>(b =>
        {
            b.HasIndex(s => s.Code).IsUnique();
            b.Property(s => s.CurrentBalance).HasPrecision(18, 2);
        });

        builder.Entity<PurchaseOrder>(b =>
        {
            b.HasIndex(p => p.OrderNumber).IsUnique();
            b.Property(p => p.SubTotal).HasPrecision(18, 2);
            b.Property(p => p.TaxAmount).HasPrecision(18, 2);
            b.Property(p => p.DiscountAmount).HasPrecision(18, 2);
            b.Property(p => p.TotalAmount).HasPrecision(18, 2);
        });

        builder.Entity<PurchaseOrderItem>(b =>
        {
            b.Property(i => i.Quantity).HasPrecision(18, 2);
            b.Property(i => i.ReceivedQuantity).HasPrecision(18, 2);
            b.Property(i => i.UnitCost).HasPrecision(18, 2);
            b.Property(i => i.Discount).HasPrecision(18, 2);
            b.Property(i => i.TaxRate).HasPrecision(18, 2);
            b.Ignore(i => i.LineTotal);
            b.HasOne(i => i.PurchaseOrder).WithMany(p => p.Items).HasForeignKey(i => i.PurchaseOrderId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(i => i.Product).WithMany().HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Restrict);
        });

        // CRM
        builder.Entity<Lead>().Property(l => l.EstimatedValue).HasPrecision(18, 2);
        builder.Entity<Opportunity>(b =>
        {
            b.Property(o => o.Amount).HasPrecision(18, 2);
            b.Property(o => o.Probability).HasPrecision(5, 2);
        });

        // Soft delete query filters
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                var prop = System.Linq.Expressions.Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                var notDeleted = System.Linq.Expressions.Expression.Equal(prop, System.Linq.Expressions.Expression.Constant(false));
                var lambda = System.Linq.Expressions.Expression.Lambda(notDeleted, parameter);
                builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var userId = _currentUser?.UserId;
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedBy = userId;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedBy = userId;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
