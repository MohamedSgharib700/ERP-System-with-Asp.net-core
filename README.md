# ERP System – ASP.NET Core 9 Web API

A complete ERP (Enterprise Resource Planning) system built with **ASP.NET Core 9 Web API** following **Clean Architecture**, using **Entity Framework Core 9**, **SQL Server**, **JWT Authentication**, and **ASP.NET Core Identity**.

## 🏛️ Architecture (Clean Architecture)

```
ErpSystem.sln
└── src/
    ├── ErpSystem.Domain         → Entities and Enums (no dependencies)
    ├── ErpSystem.Application    → DTOs, Interfaces, AutoMapper Profiles
    ├── ErpSystem.Infrastructure → DbContext, Identity, JWT, Migrations
    └── ErpSystem.API            → Controllers, Program.cs, Swagger
```

## 📦 Included Modules

| Module | Sub-modules |
|--------|-------------|
| **Identity / Auth** | Users, Roles, JWT, Login / Register / Assign Role |
| **HR** | Departments, Job Titles, Employees, Attendances, Leave Requests |
| **Finance** | Chart of Accounts, Fiscal Years, Journal Entries, Trial Balance |
| **Inventory** | Categories, Units of Measure, Warehouses, Products, Stock Movements |
| **Sales** | Customers, Sales Orders, Invoices, Payments |
| **Purchasing** | Suppliers, Purchase Orders, Goods Receipt |
| **CRM** | Leads, Contacts, Opportunities, Activities |
| **Reports** | Dashboard, Sales by Customer, Inventory Valuation, A/R Aging, Trial Balance |

## ✅ Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server (LocalDB is sufficient for development)
- (Optional) `dotnet-ef` global tool: `dotnet tool install --global dotnet-ef`

## 🚀 Getting Started

```powershell
# 1) Restore packages and build the solution
dotnet restore
dotnet build

# 2) (Optional) Apply the migrations manually
#    Or leave them to be applied automatically at API startup (DbInitializer.SeedAsync)
dotnet ef database update --project src/ErpSystem.Infrastructure --startup-project src/ErpSystem.API

# 3) Run the API
dotnet run --project src/ErpSystem.API
```

Swagger UI will open automatically at the root:
```
https://localhost:7xxx/
http://localhost:5xxx/
```



### Predefined Roles
`Admin`, `Manager`, `Accountant`, `HR`, `Sales`, `Purchasing`, `User`

## 🧪 Using Swagger

1. Call `POST /api/Auth/login` 
2. Copy the `token` value from the response.
3. Click the **Authorize** button in Swagger and paste: `Bearer {token}` (or just the token).
4. You can now call all the protected endpoints.

## ⚙️ Configuration (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ErpSystemDb;Trusted_Connection=True;..."
  },
  "JwtSettings": {
    "Issuer": "ErpSystem",
    "Audience": "ErpSystemClients",
    "Key": "ThisIsADevelopmentJwtKey_ChangeInProduction_MustBeAtLeast32Chars!!",
    "ExpiryMinutes": 240
  }
}
```

> ⚠️ **Important for production:** Change `JwtSettings:Key` (at least 32 characters) and use User Secrets or environment variables.

## 📑 Key API Endpoints

| Resource | Method | Description |
|----------|--------|-------------|
| `/api/Auth/login` | POST | Login |
| `/api/Auth/register` | POST | Register a new user |
| `/api/Departments` | CRUD | Departments |
| `/api/Employees` | CRUD | Employees |
| `/api/LeaveRequests/{id}/approve` | POST | Approve a leave request |
| `/api/Accounts` | CRUD | Chart of accounts |
| `/api/JournalEntries` | CRUD | Journal entries |
| `/api/JournalEntries/{id}/post` | POST | Post a journal entry |
| `/api/JournalEntries/trial-balance` | GET | Trial balance |
| `/api/Products` | CRUD | Products |
| `/api/Products/low-stock` | GET | Products at or below reorder level |
| `/api/StockMovements` | GET / POST | Stock movements |
| `/api/Customers` | CRUD | Customers |
| `/api/SalesOrders` | CRUD + confirm/cancel | Sales orders |
| `/api/Invoices` | CRUD | Invoices |
| `/api/Invoices/payment` | POST | Record a payment |
| `/api/Suppliers` | CRUD | Suppliers |
| `/api/PurchaseOrders` | CRUD | Purchase orders |
| `/api/PurchaseOrders/receive` | POST | Receive goods |
| `/api/Leads` `/api/Contacts` `/api/Opportunities` `/api/Activities` | CRUD | CRM |
| `/api/Reports/dashboard` | GET | KPI dashboard |
| `/api/Reports/sales-by-customer` | GET | Sales by customer |
| `/api/Reports/inventory-valuation` | GET | Inventory valuation |
| `/api/Reports/accounts-receivable` | GET | Accounts receivable |

## 🏗️ Additional Features

- **Soft Delete** — Logical deletion via `IsDeleted` together with global query filters.
- **Automatic Auditing** — `CreatedAt/By` and `UpdatedAt/By` are populated in `SaveChangesAsync`.
- **JWT Auth + Role-Based Authorization** at the endpoint level.
- **AutoMapper** — Automatic mapping between entities and DTOs.
- **Swagger** with built-in Authorize button support.
- **Generic Pagination** via `PagedResult<T>` and `PagedQuery`.
- **CORS** open for development.
- **Automatic Seeding** (admin user, chart of accounts, units of measure, departments, etc.).
---


