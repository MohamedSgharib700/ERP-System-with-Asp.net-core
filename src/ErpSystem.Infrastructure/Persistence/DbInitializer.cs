using ErpSystem.Domain.Enums;
using ErpSystem.Domain.Finance;
using ErpSystem.Domain.HR;
using ErpSystem.Domain.Inventory;
using ErpSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        await db.Database.MigrateAsync();

        // Roles
        string[] roles = { "Admin", "Manager", "Accountant", "HR", "Sales", "Purchasing", "User" };
        foreach (var r in roles)
        {
            if (!await roleManager.RoleExistsAsync(r))
                await roleManager.CreateAsync(new ApplicationRole { Name = r, Description = r + " Role" });
        }

        // Admin user
        var admin = await userManager.FindByNameAsync("admin");
        if (admin == null)
        {
            admin = new ApplicationUser { UserName = "admin", Email = "admin@erp.local", FullName = "System Administrator", EmailConfirmed = true, IsActive = true };
            var res = await userManager.CreateAsync(admin, "Admin@123");
            if (res.Succeeded) await userManager.AddToRoleAsync(admin, "Admin");
        }

        // Sample HR data
        if (!await db.Departments.AnyAsync())
        {
            db.Departments.AddRange(
                new Department { Name = "Administration", Code = "ADM" },
                new Department { Name = "Human Resources", Code = "HR" },
                new Department { Name = "Finance", Code = "FIN" },
                new Department { Name = "Sales", Code = "SAL" },
                new Department { Name = "Purchasing", Code = "PUR" },
                new Department { Name = "IT", Code = "IT" }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.JobTitles.AnyAsync())
        {
            db.JobTitles.AddRange(
                new JobTitle { Title = "Manager", MinSalary = 5000, MaxSalary = 15000 },
                new JobTitle { Title = "Senior Developer", MinSalary = 4000, MaxSalary = 10000 },
                new JobTitle { Title = "Accountant", MinSalary = 2500, MaxSalary = 6000 },
                new JobTitle { Title = "Sales Representative", MinSalary = 2000, MaxSalary = 5000 }
            );
            await db.SaveChangesAsync();
        }

        // Finance: Chart of accounts
        if (!await db.FiscalYears.AnyAsync())
        {
            db.FiscalYears.Add(new FiscalYear
            {
                Name = $"FY{DateTime.UtcNow.Year}",
                StartDate = new DateTime(DateTime.UtcNow.Year, 1, 1),
                EndDate = new DateTime(DateTime.UtcNow.Year, 12, 31)
            });
            await db.SaveChangesAsync();
        }

        if (!await db.Accounts.AnyAsync())
        {
            db.Accounts.AddRange(
                new Account { Code = "1000", Name = "Assets", Type = AccountType.Asset },
                new Account { Code = "1100", Name = "Cash", Type = AccountType.Asset },
                new Account { Code = "1200", Name = "Accounts Receivable", Type = AccountType.Asset },
                new Account { Code = "1300", Name = "Inventory", Type = AccountType.Asset },
                new Account { Code = "2000", Name = "Liabilities", Type = AccountType.Liability },
                new Account { Code = "2100", Name = "Accounts Payable", Type = AccountType.Liability },
                new Account { Code = "3000", Name = "Equity", Type = AccountType.Equity },
                new Account { Code = "4000", Name = "Sales Revenue", Type = AccountType.Revenue },
                new Account { Code = "5000", Name = "Cost of Goods Sold", Type = AccountType.Expense },
                new Account { Code = "5100", Name = "Salaries Expense", Type = AccountType.Expense }
            );
            await db.SaveChangesAsync();
        }

        // Inventory base
        if (!await db.UnitsOfMeasure.AnyAsync())
        {
            db.UnitsOfMeasure.AddRange(
                new UnitOfMeasure { Name = "Piece", Symbol = "PC" },
                new UnitOfMeasure { Name = "Kilogram", Symbol = "KG" },
                new UnitOfMeasure { Name = "Liter", Symbol = "L" },
                new UnitOfMeasure { Name = "Box", Symbol = "BOX" }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Categories.AnyAsync())
        {
            db.Categories.AddRange(
                new Category { Name = "Electronics" },
                new Category { Name = "Office Supplies" },
                new Category { Name = "Raw Materials" }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Warehouses.AnyAsync())
        {
            db.Warehouses.Add(new Warehouse { Code = "MAIN", Name = "Main Warehouse", Address = "HQ", IsActive = true });
            await db.SaveChangesAsync();
        }
    }
}
