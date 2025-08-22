# ?? Stock Room Management System - Complete Setup Guide

## ?? Overview

This is a modern, comprehensive inventory management system built with .NET 8 Windows Forms, featuring:

- ? **Modern UI Design** with FontAwesome icons
- ?? **User Authentication** with role-based access
- ?? **Full CRUD Operations** for inventory items
- ?? **Dashboard Analytics** with real-time statistics
- ??? **Category & Supplier Management**
- ?? **Stock Movement Tracking**
- ?? **Advanced Search & Filtering**
- ?? **MS Access Database** with comprehensive schema

---

## ?? Quick Start

### Prerequisites
- ? Windows 10/11
- ? .NET 8 Runtime
- ? Microsoft Access Database Engine (ACE.OLEDB.12.0)
- ? Visual Studio 2022 or VS Code (for development)

### Installation Steps

1. **Clone or Download** the repository
2. **Open** `Stock Room.csproj` in Visual Studio
3. **Build** the solution (`Ctrl+Shift+B`)
4. **Set up the database** (see Database Setup section)
5. **Run** the application (`F5`)

---

## ?? Database Setup

### Step 1: Install Microsoft Access Database Engine
Download and install: [Microsoft Access Database Engine 2019 Redistributable](https://www.microsoft.com/en-us/download/details.aspx?id=54920)

### Step 2: Create Database File
1. Create folder: `[Application Directory]/Database/`
2. Create new Access database: `StockRoom.accdb`
3. Run the SQL scripts from `Database/DatabaseSetupGuide.md`

### Step 3: Database Schema
The system uses 6 main tables:

#### **Users Table**
```sql
CREATE TABLE Users (
    UserID AUTOINCREMENT PRIMARY KEY,
    Username TEXT(50) NOT NULL,
    PasswordHash TEXT(255) NOT NULL,
    FullName TEXT(100) NOT NULL,
    Email TEXT(100),
    Role TEXT(20) DEFAULT "User" NOT NULL,
    CreatedDate DATETIME DEFAULT Now() NOT NULL,
    Status TEXT(20) DEFAULT "Active" NOT NULL
);
```

#### **Categories Table**
```sql
CREATE TABLE Categories (
    CategoryID AUTOINCREMENT PRIMARY KEY,
    CategoryName TEXT(100) NOT NULL,
    Description MEMO,
    CreatedDate DATETIME DEFAULT Now() NOT NULL,
    Status TEXT(20) DEFAULT "Active" NOT NULL
);
```

#### **Suppliers Table**
```sql
CREATE TABLE Suppliers (
    SupplierID AUTOINCREMENT PRIMARY KEY,
    SupplierName TEXT(100) NOT NULL,
    ContactPerson TEXT(100),
    Phone TEXT(15),
    Email TEXT(100),
    Address TEXT(255),
    CreatedDate DATETIME DEFAULT Now() NOT NULL,
    Status TEXT(20) DEFAULT "Active" NOT NULL
);
```

#### **InventoryItems Table**
```sql
CREATE TABLE InventoryItems (
    ItemID AUTOINCREMENT PRIMARY KEY,
    ItemName TEXT(100) NOT NULL,
    Description MEMO,
    SKU TEXT(50),
    CategoryID LONG NOT NULL,
    SupplierID LONG NOT NULL,
    Quantity LONG DEFAULT 0 NOT NULL,
    MinimumStock LONG DEFAULT 0 NOT NULL,
    CostPrice CURRENCY DEFAULT 0 NOT NULL,
    SellingPrice CURRENCY DEFAULT 0 NOT NULL,
    CreatedDate DATETIME DEFAULT Now() NOT NULL,
    Status TEXT(20) DEFAULT "Active" NOT NULL
);
```

#### **StockMovements Table**
```sql
CREATE TABLE StockMovements (
    MovementID AUTOINCREMENT PRIMARY KEY,
    ItemID LONG NOT NULL,
    MovementType TEXT(20) NOT NULL,
    Quantity LONG NOT NULL,
    MovementDate DATETIME DEFAULT Now() NOT NULL,
    UserID LONG NOT NULL
);
```

#### **AuditLog Table**
```sql
CREATE TABLE AuditLog (
    LogID AUTOINCREMENT PRIMARY KEY,
    UserID LONG NOT NULL,
    Action TEXT(50) NOT NULL,
    TableName TEXT(50) NOT NULL,
    Timestamp DATETIME DEFAULT Now() NOT NULL
);
```

### Step 4: Insert Sample Data

#### Default Admin User:
```sql
INSERT INTO Users (Username, PasswordHash, FullName, Role, Status)
VALUES ('admin', 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', 'System Administrator', 'Admin', 'Active');
```
**Password:** `admin123`

#### Sample Categories:
```sql
INSERT INTO Categories (CategoryName, Description) VALUES 
('Electronics', 'Electronic devices and components'),
('Office Supplies', 'General office supplies and stationery'),
('Tools', 'Hand tools and equipment');
```

#### Sample Suppliers:
```sql
INSERT INTO Suppliers (SupplierName, ContactPerson, Email) VALUES 
('Tech Solutions Inc.', 'John Smith', 'john@techsolutions.com'),
('Office Pro Supply', 'Mary Johnson', 'mary@officepro.com');
```

---

## ??? System Features

### ?? Login System
- **Modern UI** with gradient backgrounds
- **Role-based authentication** (Admin, Manager, User, Viewer)
- **Demo credentials** displayed on login screen
- **Password hashing** with SHA256 + salt
- **Session management** with last login tracking

### ?? Dashboard
- **Real-time statistics** cards showing:
  - Total inventory items
  - Low stock alerts
  - Categories count
  - Total inventory value
- **Recent activity feed**
- **Quick navigation** to all modules
- **Responsive design** that adapts to screen size

### ?? Inventory Management
- **Add/Edit/Delete** inventory items
- **Advanced search** and filtering
- **Bulk operations** support
- **Stock level monitoring**
- **Category and supplier assignment**
- **Pricing management** with profit calculation
- **Barcode support**
- **Location tracking**

### ??? Category Management
- **Organize items** into categories
- **Hierarchical structure** support
- **Item count** per category
- **Status management** (Active/Inactive)

### ?? Supplier Management  
- **Complete supplier profiles**
- **Contact information** management
- **Performance tracking**
- **Item assignment** to suppliers

### ?? Stock Movement Tracking
- **Real-time stock movements**
- **Movement types**: In, Out, Adjustment, Transfer
- **Reference numbers** and notes
- **User audit trail**
- **Date/time stamping**

### ?? Advanced Search & Filtering
- **Multi-field search** across items
- **Filter by category** and supplier
- **Status-based filtering**
- **Date range queries**
- **Sort by multiple fields**
- **Pagination** for large datasets

---

## ?? UI Components

### Modern Design Elements
- **FontAwesome Icons** integration
- **Material Design** inspired colors
- **Gradient backgrounds** and shadows
- **Rounded corners** and modern typography
- **Responsive layouts** with proper anchoring

### Custom Controls
- **ModernButton**: Styled buttons with hover effects
- **ModernTextBox**: Enhanced text input with placeholders
- **ModernComboBox**: Styled dropdown controls
- **ModernPanel**: Cards with shadow effects
- **ModernDataGridView**: Enhanced data grid with custom styling

### Color Scheme
```csharp
// Primary Colors
Primary: #2563EB (Blue)
PrimaryDark: #1D4ED8
PrimaryLight: #3B82F6

// Status Colors  
Success: #22C55E (Green)
Warning: #F59E0B (Orange)
Danger: #EF4444 (Red)
Info: #3B82F6 (Blue)

// Neutral Colors
Background: #F8FAFCF
Surface: #FFFFFF
TextPrimary: #0F172A
TextSecondary: #475569
```

---

## ?? Configuration

### Connection String
The system automatically looks for the database in:
```
[Application Directory]/Database/StockRoom.accdb
```

To use a different location, modify `DatabaseHelper.cs`:
```csharp
string dbPath = "C:\\Your\\Custom\\Path\\StockRoom.accdb";
connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};";
```

### FontAwesome Icons
Update the FontAwesome path in `ResourceManager.cs`:
```csharp
private static readonly string FontAwesomePath = @"X:\c\resources\icon\fontawesome-free-7.0.0-desktop";
```

---

## ????? Development Guide

### Project Structure
```
Stock Room Management System/
??? ?? Models/
?   ??? DataModels.cs          # Data models and entities
??? ?? UI/
?   ??? ModernUIComponents.cs  # Custom UI controls
??? ?? Forms/
?   ??? InventoryItemForm.cs   # CRUD forms
??? ?? Resources/
?   ??? Icons/                 # Application icons
?   ??? Images/                # Application images
?   ??? *.md                   # Documentation
??? ?? Database/
?   ??? DatabaseSetupGuide.md  # Database documentation
??? DatabaseHelper.cs          # Database operations
??? ResourceManager.cs         # Resource management
??? LoginForm.cs              # Authentication UI
??? DashboardForm.cs          # Main application UI
??? Program.cs                # Application entry point
```

### Key Classes

#### **DatabaseHelper**
- Async database operations
- Connection management
- CRUD operations for all entities
- Error handling and logging
- SQL injection prevention

#### **ResourceManager**
- Theme and styling constants
- FontAwesome icon management
- Resource loading and caching
- Fallback icon generation

#### **Models**
- **InventoryItem**: Core inventory entity
- **Category**: Item categorization
- **Supplier**: Vendor management
- **User**: System users
- **StockMovement**: Inventory tracking
- **AuditLog**: System audit trail

### Adding New Features

#### 1. New Entity
```csharp
// Add to Models/DataModels.cs
public class NewEntity
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    // ... other properties
}
```

#### 2. Database Operations
```csharp
// Add to DatabaseHelper.cs
public async Task<List<NewEntity>> GetNewEntitiesAsync()
{
    // Implementation
}

public async Task<int> CreateNewEntityAsync(NewEntity entity)
{
    // Implementation
}
```

#### 3. UI Form
```csharp
// Create Forms/NewEntityForm.cs
public partial class NewEntityForm : Form
{
    // Modern UI implementation
}
```

---

## ?? User Roles & Permissions

### ?? Administrator
- **Full system access**
- User management
- System configuration
- Database maintenance
- All CRUD operations

### ????? Manager  
- **Inventory management**
- Reports and analytics
- Category/supplier management
- Most CRUD operations
- Cannot manage users

### ?? User
- **Basic inventory operations**
- Add/edit inventory items
- View reports
- Limited system access

### ??? Viewer
- **Read-only access**
- View inventory
- View reports
- Cannot modify data

---

## ?? Getting Started Checklist

- [ ] ? Install .NET 8 Runtime
- [ ] ? Install Microsoft Access Database Engine  
- [ ] ? Create Database folder
- [ ] ? Set up StockRoom.accdb database
- [ ] ? Run SQL scripts to create tables
- [ ] ? Insert sample data
- [ ] ? Configure FontAwesome path (optional)
- [ ] ? Build and run application
- [ ] ? Login with admin credentials
- [ ] ? Explore the dashboard
- [ ] ? Add your first inventory item

---

## ?? Troubleshooting

### Database Connection Issues
**Problem**: "Could not find database file"
**Solution**: Ensure `StockRoom.accdb` exists in `Database/` folder

**Problem**: "Provider not found"  
**Solution**: Install Microsoft Access Database Engine

### FontAwesome Icons Not Loading
**Problem**: Icons show as placeholders
**Solution**: Update `FontAwesomePath` in `ResourceManager.cs`

### Build Errors
**Problem**: Missing dependencies
**Solution**: Restore NuGet packages: `dotnet restore`

### Performance Issues
**Problem**: Slow database queries
**Solution**: Add indexes to frequently queried fields

---

## ?? Support

For issues or questions:
1. Check the troubleshooting section
2. Review the database setup guide
3. Examine the source code documentation
4. Create an issue in the repository

---

## ?? Future Enhancements

### Planned Features
- ?? **Mobile app** companion
- ?? **Cloud sync** and backup
- ?? **Advanced analytics** and reporting
- ?? **Email notifications** for low stock
- ?? **Barcode scanner** integration  
- ?? **Data export/import** functionality
- ?? **Multi-language** support
- ?? **API endpoints** for third-party integration

### Technical Improvements
- ??? **SQL Server** support
- ?? **Real-time updates** with SignalR
- ?? **Performance optimization**
- ?? **Unit testing** framework
- ?? **Deployment automation**
- ?? **Telemetry and monitoring**

---

**?? Congratulations! You now have a fully functional, modern inventory management system!**

*Happy inventory managing! ???*