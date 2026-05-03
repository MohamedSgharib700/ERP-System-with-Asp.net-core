namespace ErpSystem.Domain.Enums;

public enum Gender { Male = 1, Female = 2 }

public enum EmployeeStatus { Active = 1, OnLeave = 2, Terminated = 3 }

public enum LeaveType { Annual = 1, Sick = 2, Unpaid = 3, Maternity = 4, Other = 5 }

public enum LeaveStatus { Pending = 1, Approved = 2, Rejected = 3 }

public enum AttendanceStatus { Present = 1, Absent = 2, Late = 3, OnLeave = 4 }

public enum AccountType { Asset = 1, Liability = 2, Equity = 3, Revenue = 4, Expense = 5 }

public enum JournalStatus { Draft = 1, Posted = 2, Cancelled = 3 }

public enum StockMovementType { In = 1, Out = 2, Transfer = 3, Adjustment = 4 }

public enum SalesOrderStatus { Draft = 1, Confirmed = 2, Shipped = 3, Delivered = 4, Cancelled = 5 }

public enum InvoiceStatus { Draft = 1, Issued = 2, PartiallyPaid = 3, Paid = 4, Overdue = 5, Cancelled = 6 }

public enum PurchaseOrderStatus { Draft = 1, Sent = 2, Received = 3, Cancelled = 4 }

public enum LeadStatus { New = 1, Contacted = 2, Qualified = 3, Lost = 4, Converted = 5 }

public enum OpportunityStage { Prospecting = 1, Qualification = 2, Proposal = 3, Negotiation = 4, Won = 5, Lost = 6 }

public enum ActivityType { Call = 1, Email = 2, Meeting = 3, Task = 4, Note = 5 }
