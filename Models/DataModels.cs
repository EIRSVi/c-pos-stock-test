using System.ComponentModel.DataAnnotations;

namespace Stock_Room.Models
{
    /// <summary>
    /// Inventory item model for the stock management system
    /// </summary>
    public class InventoryItem
    {
        public int ItemID { get; set; }
        
        public string ItemName { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public string SKU { get; set; } = string.Empty;
        
        public string Barcode { get; set; } = string.Empty;
        
        public int CategoryID { get; set; }
        
        public int SupplierID { get; set; }
        
        public int Quantity { get; set; }
        
        public int MinimumStock { get; set; }
        
        public int ReorderLevel { get; set; }
        
        public string Unit { get; set; } = "pcs";
        
        public decimal CostPrice { get; set; }
        
        public decimal SellingPrice { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public DateTime? LastUpdated { get; set; }
        
        public DateTime? LastRestocked { get; set; }
        
        public string Status { get; set; } = "Active"; // Active, Inactive, Discontinued
        
        public string Location { get; set; } = string.Empty;
        
        public string Notes { get; set; } = string.Empty;

        // Navigation properties (will be set from database)
        public string? CategoryName { get; set; }
        public string? SupplierName { get; set; }

        // Computed properties
        public bool IsLowStock => Quantity <= MinimumStock;
        public bool NeedsReorder => Quantity <= ReorderLevel;
        public decimal TotalValue => Quantity * CostPrice;
        public decimal ProfitMargin => SellingPrice - CostPrice;
        public double ProfitPercentage => CostPrice > 0 ? (double)((SellingPrice - CostPrice) / CostPrice * 100) : 0;
    }

    /// <summary>
    /// Category model for organizing inventory items
    /// </summary>
    public class Category
    {
        public int CategoryID { get; set; }
        
        public string CategoryName { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public string Status { get; set; } = "Active";
        
        // Computed property
        public int ItemCount { get; set; }
    }

    /// <summary>
    /// Supplier model for vendor management
    /// </summary>
    public class Supplier
    {
        public int SupplierID { get; set; }
        
        public string SupplierName { get; set; } = string.Empty;
        
        public string ContactPerson { get; set; } = string.Empty;
        
        public string Phone { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;
        
        public string Address { get; set; } = string.Empty;
        
        public string City { get; set; } = string.Empty;
        
        public string PostalCode { get; set; } = string.Empty;
        
        public string Country { get; set; } = string.Empty;
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public string Status { get; set; } = "Active";
        
        public string Notes { get; set; } = string.Empty;
        
        // Computed property
        public int ItemCount { get; set; }
    }

    /// <summary>
    /// Stock movement model for tracking inventory changes
    /// </summary>
    public class StockMovement
    {
        public int MovementID { get; set; }
        
        public int ItemID { get; set; }
        
        public string MovementType { get; set; } = string.Empty; // In, Out, Adjustment, Transfer
        
        public int Quantity { get; set; }
        
        public string Reference { get; set; } = string.Empty; // PO Number, Invoice Number, etc.
        
        public string Reason { get; set; } = string.Empty;
        
        public DateTime MovementDate { get; set; } = DateTime.Now;
        
        public int UserID { get; set; }
        
        public string Notes { get; set; } = string.Empty;
        
        // Navigation properties
        public string? ItemName { get; set; }
        public string? UserName { get; set; }
    }

    /// <summary>
    /// User model for system authentication and authorization
    /// </summary>
    public class User
    {
        public int UserID { get; set; }
        
        public string Username { get; set; } = string.Empty;
        
        public string PasswordHash { get; set; } = string.Empty;
        
        public string FullName { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;
        
        public string Phone { get; set; } = string.Empty;
        
        public string Role { get; set; } = "User"; // Admin, Manager, User, Viewer
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public DateTime? LastLogin { get; set; }
        
        public string Status { get; set; } = "Active";
        
        public string Notes { get; set; } = string.Empty;

        // Helper methods for role checking
        public bool IsAdmin => Role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        public bool IsManager => Role.Equals("Manager", StringComparison.OrdinalIgnoreCase) || IsAdmin;
        public bool CanEdit => IsManager || Role.Equals("User", StringComparison.OrdinalIgnoreCase);
        public bool CanView => !string.IsNullOrEmpty(Role);
    }

    /// <summary>
    /// Audit log model for tracking system changes
    /// </summary>
    public class AuditLog
    {
        public int LogID { get; set; }
        
        public int UserID { get; set; }
        
        public string Action { get; set; } = string.Empty; // Create, Update, Delete, Login, Logout
        
        public string TableName { get; set; } = string.Empty;
        
        public int? RecordID { get; set; }
        
        public string OldValues { get; set; } = string.Empty;
        
        public string NewValues { get; set; } = string.Empty;
        
        public DateTime Timestamp { get; set; } = DateTime.Now;
        
        public string IPAddress { get; set; } = string.Empty;
        
        public string Notes { get; set; } = string.Empty;
        
        // Navigation property
        public string? UserName { get; set; }
    }

    /// <summary>
    /// Dashboard statistics model
    /// </summary>
    public class DashboardStats
    {
        public int TotalItems { get; set; }
        public int LowStockItems { get; set; }
        public int OutOfStockItems { get; set; }
        public int TotalCategories { get; set; }
        public int TotalSuppliers { get; set; }
        public int ActiveUsers { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public int RecentMovements { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Search and filter parameters for inventory queries
    /// </summary>
    public class InventorySearchParams
    {
        public string? SearchTerm { get; set; }
        public int? CategoryID { get; set; }
        public int? SupplierID { get; set; }
        public string? Status { get; set; }
        public bool? LowStockOnly { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public string SortBy { get; set; } = "ItemName";
        public string SortOrder { get; set; } = "ASC";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}