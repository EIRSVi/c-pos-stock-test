# Stock Room Management System - Database Setup Guide

## Microsoft Access Database Structure

### Database File Location
Create a new Microsoft Access database file:
- **File Name**: `StockRoom.accdb`
- **Location**: `[Application Directory]/Database/StockRoom.accdb`
- **Format**: Access 2007-2019 (.accdb)

---

## Table Structures

### 1. Users Table
Stores user account information for system authentication.

**Table Name**: `Users`

| Field Name | Data Type | Field Size | Required | Default Value | Description |
|------------|-----------|------------|----------|---------------|-------------|
| UserID | AutoNumber | Long Integer | Yes | | Primary Key |
| Username | Text | 50 | Yes | | Login username |
| PasswordHash | Text | 255 | Yes | | SHA256 hashed password |
| FullName | Text | 100 | Yes | | User's full name |
| Email | Text | 100 | No | | Email address |
| Phone | Text | 15 | No | | Phone number |
| Role | Text | 20 | Yes | "User" | User role (Admin, Manager, User, Viewer) |
| CreatedDate | Date/Time | | Yes | Now() | Account creation date |
| LastLogin | Date/Time | | No | | Last login timestamp |
| Status | Text | 20 | Yes | "Active" | Account status |
| Notes | Memo | | No | | Additional notes |

**Indexes**:
- Primary Key: UserID
- Unique Index: Username

**Sample Data**:
```
Username: admin
Password: admin123 (will be hashed)
FullName: System Administrator
Role: Admin
Status: Active
```

---

### 2. Categories Table
Organizes inventory items into categories.

**Table Name**: `Categories`

| Field Name | Data Type | Field Size | Required | Default Value | Description |
|------------|-----------|------------|----------|---------------|-------------|
| CategoryID | AutoNumber | Long Integer | Yes | | Primary Key |
| CategoryName | Text | 100 | Yes | | Category name |
| Description | Memo | | No | | Category description |
| CreatedDate | Date/Time | | Yes | Now() | Creation date |
| Status | Text | 20 | Yes | "Active" | Category status |

**Indexes**:
- Primary Key: CategoryID
- Index: CategoryName

**Sample Data**:
```
CategoryName: Electronics
Description: Electronic devices and components
Status: Active

CategoryName: Office Supplies
Description: General office supplies and stationery
Status: Active

CategoryName: Tools
Description: Hand tools and equipment
Status: Active
```

---

### 3. Suppliers Table
Manages supplier/vendor information.

**Table Name**: `Suppliers`

| Field Name | Data Type | Field Size | Required | Default Value | Description |
|------------|-----------|------------|----------|---------------|-------------|
| SupplierID | AutoNumber | Long Integer | Yes | | Primary Key |
| SupplierName | Text | 100 | Yes | | Supplier company name |
| ContactPerson | Text | 100 | No | | Contact person name |
| Phone | Text | 15 | No | | Phone number |
| Email | Text | 100 | No | | Email address |
| Address | Text | 255 | No | | Street address |
| City | Text | 50 | No | | City |
| PostalCode | Text | 10 | No | | Postal/ZIP code |
| Country | Text | 50 | No | | Country |
| CreatedDate | Date/Time | | Yes | Now() | Creation date |
| Status | Text | 20 | Yes | "Active" | Supplier status |
| Notes | Memo | | No | | Additional notes |

**Indexes**:
- Primary Key: SupplierID
- Index: SupplierName

**Sample Data**:
```
SupplierName: Tech Solutions Inc.
ContactPerson: John Smith
Phone: +1-555-0123
Email: john@techsolutions.com
City: New York
Country: USA
Status: Active
```

---

### 4. InventoryItems Table
Main inventory items storage.

**Table Name**: `InventoryItems`

| Field Name | Data Type | Field Size | Required | Default Value | Description |
|------------|-----------|------------|----------|---------------|-------------|
| ItemID | AutoNumber | Long Integer | Yes | | Primary Key |
| ItemName | Text | 100 | Yes | | Item name |
| Description | Memo | | No | | Item description |
| SKU | Text | 50 | No | | Stock Keeping Unit |
| Barcode | Text | 50 | No | | Barcode number |
| CategoryID | Number | Long Integer | Yes | | Foreign Key to Categories |
| SupplierID | Number | Long Integer | Yes | | Foreign Key to Suppliers |
| Quantity | Number | Long Integer | Yes | 0 | Current stock quantity |
| MinimumStock | Number | Long Integer | Yes | 0 | Minimum stock level |
| ReorderLevel | Number | Long Integer | Yes | 0 | Reorder point |
| Unit | Text | 20 | Yes | "pcs" | Unit of measurement |
| CostPrice | Currency | | Yes | 0 | Cost per unit |
| SellingPrice | Currency | | Yes | 0 | Selling price per unit |
| CreatedDate | Date/Time | | Yes | Now() | Creation date |
| LastUpdated | Date/Time | | No | | Last update timestamp |
| LastRestocked | Date/Time | | No | | Last restock date |
| Status | Text | 20 | Yes | "Active" | Item status |
| Location | Text | 255 | No | | Storage location |
| Notes | Memo | | No | | Additional notes |

**Relationships**:
- CategoryID ? Categories.CategoryID (One-to-Many)
- SupplierID ? Suppliers.SupplierID (One-to-Many)

**Indexes**:
- Primary Key: ItemID
- Index: ItemName
- Index: SKU
- Index: Barcode

**Sample Data**:
```
ItemName: Wireless Mouse
SKU: WM-001
CategoryID: 1 (Electronics)
SupplierID: 1 (Tech Solutions Inc.)
Quantity: 50
MinimumStock: 10
ReorderLevel: 15
CostPrice: $25.00
SellingPrice: $39.99
Status: Active
```

---

### 5. StockMovements Table
Tracks all stock movements (in, out, adjustments).

**Table Name**: `StockMovements`

| Field Name | Data Type | Field Size | Required | Default Value | Description |
|------------|-----------|------------|----------|---------------|-------------|
| MovementID | AutoNumber | Long Integer | Yes | | Primary Key |
| ItemID | Number | Long Integer | Yes | | Foreign Key to InventoryItems |
| MovementType | Text | 20 | Yes | | Type: In, Out, Adjustment, Transfer |
| Quantity | Number | Long Integer | Yes | | Quantity moved (+ or -) |
| Reference | Text | 100 | No | | Reference number (PO, Invoice, etc.) |
| Reason | Memo | | No | | Reason for movement |
| MovementDate | Date/Time | | Yes | Now() | Movement date |
| UserID | Number | Long Integer | Yes | | User who made the change |
| Notes | Memo | | No | | Additional notes |

**Relationships**:
- ItemID ? InventoryItems.ItemID (One-to-Many)
- UserID ? Users.UserID (One-to-Many)

**Indexes**:
- Primary Key: MovementID
- Index: ItemID
- Index: MovementDate

**Sample Movement Types**:
- `In`: Stock received from supplier
- `Out`: Stock sold or issued
- `Adjustment`: Inventory count adjustment
- `Transfer`: Stock transfer between locations

---

### 6. AuditLog Table
System audit trail for all changes.

**Table Name**: `AuditLog`

| Field Name | Data Type | Field Size | Required | Default Value | Description |
|------------|-----------|------------|----------|---------------|-------------|
| LogID | AutoNumber | Long Integer | Yes | | Primary Key |
| UserID | Number | Long Integer | Yes | | User who performed action |
| Action | Text | 50 | Yes | | Action performed |
| TableName | Text | 50 | Yes | | Table affected |
| RecordID | Number | Long Integer | No | | ID of affected record |
| OldValues | Memo | | No | | Previous values (JSON format) |
| NewValues | Memo | | No | | New values (JSON format) |
| Timestamp | Date/Time | | Yes | Now() | When action occurred |
| IPAddress | Text | 45 | No | | User's IP address |
| Notes | Memo | | No | | Additional notes |

**Relationships**:
- UserID ? Users.UserID (One-to-Many)

**Indexes**:
- Primary Key: LogID
- Index: Timestamp
- Index: UserID

---

## Database Setup Instructions

### Step 1: Create Database File
1. Open Microsoft Access
2. Create a new blank database
3. Save as `StockRoom.accdb` in your application's `Database` folder

### Step 2: Create Tables
Use the following SQL commands in Access Query Design view:

#### Create Users Table:
```sql
CREATE TABLE Users (
    UserID AUTOINCREMENT PRIMARY KEY,
    Username TEXT(50) NOT NULL,
    PasswordHash TEXT(255) NOT NULL,
    FullName TEXT(100) NOT NULL,
    Email TEXT(100),
    Phone TEXT(15),
    Role TEXT(20) DEFAULT "User" NOT NULL,
    CreatedDate DATETIME DEFAULT Now() NOT NULL,
    LastLogin DATETIME,
    Status TEXT(20) DEFAULT "Active" NOT NULL,
    Notes MEMO
);

CREATE UNIQUE INDEX IX_Users_Username ON Users (Username);
```

#### Create Categories Table:
```sql
CREATE TABLE Categories (
    CategoryID AUTOINCREMENT PRIMARY KEY,
    CategoryName TEXT(100) NOT NULL,
    Description MEMO,
    CreatedDate DATETIME DEFAULT Now() NOT NULL,
    Status TEXT(20) DEFAULT "Active" NOT NULL
);

CREATE INDEX IX_Categories_Name ON Categories (CategoryName);
```

#### Create Suppliers Table:
```sql
CREATE TABLE Suppliers (
    SupplierID AUTOINCREMENT PRIMARY KEY,
    SupplierName TEXT(100) NOT NULL,
    ContactPerson TEXT(100),
    Phone TEXT(15),
    Email TEXT(100),
    Address TEXT(255),
    City TEXT(50),
    PostalCode TEXT(10),
    Country TEXT(50),
    CreatedDate DATETIME DEFAULT Now() NOT NULL,
    Status TEXT(20) DEFAULT "Active" NOT NULL,
    Notes MEMO
);

CREATE INDEX IX_Suppliers_Name ON Suppliers (SupplierName);
```

#### Create InventoryItems Table:
```sql
CREATE TABLE InventoryItems (
    ItemID AUTOINCREMENT PRIMARY KEY,
    ItemName TEXT(100) NOT NULL,
    Description MEMO,
    SKU TEXT(50),
    Barcode TEXT(50),
    CategoryID LONG NOT NULL,
    SupplierID LONG NOT NULL,
    Quantity LONG DEFAULT 0 NOT NULL,
    MinimumStock LONG DEFAULT 0 NOT NULL,
    ReorderLevel LONG DEFAULT 0 NOT NULL,
    Unit TEXT(20) DEFAULT "pcs" NOT NULL,
    CostPrice CURRENCY DEFAULT 0 NOT NULL,
    SellingPrice CURRENCY DEFAULT 0 NOT NULL,
    CreatedDate DATETIME DEFAULT Now() NOT NULL,
    LastUpdated DATETIME,
    LastRestocked DATETIME,
    Status TEXT(20) DEFAULT "Active" NOT NULL,
    Location TEXT(255),
    Notes MEMO,
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),
    FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID)
);

CREATE INDEX IX_Items_Name ON InventoryItems (ItemName);
CREATE INDEX IX_Items_SKU ON InventoryItems (SKU);
CREATE INDEX IX_Items_Barcode ON InventoryItems (Barcode);
```

#### Create StockMovements Table:
```sql
CREATE TABLE StockMovements (
    MovementID AUTOINCREMENT PRIMARY KEY,
    ItemID LONG NOT NULL,
    MovementType TEXT(20) NOT NULL,
    Quantity LONG NOT NULL,
    Reference TEXT(100),
    Reason MEMO,
    MovementDate DATETIME DEFAULT Now() NOT NULL,
    UserID LONG NOT NULL,
    Notes MEMO,
    FOREIGN KEY (ItemID) REFERENCES InventoryItems(ItemID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

CREATE INDEX IX_Movements_Item ON StockMovements (ItemID);
CREATE INDEX IX_Movements_Date ON StockMovements (MovementDate);
```

#### Create AuditLog Table:
```sql
CREATE TABLE AuditLog (
    LogID AUTOINCREMENT PRIMARY KEY,
    UserID LONG NOT NULL,
    Action TEXT(50) NOT NULL,
    TableName TEXT(50) NOT NULL,
    RecordID LONG,
    OldValues MEMO,
    NewValues MEMO,
    Timestamp DATETIME DEFAULT Now() NOT NULL,
    IPAddress TEXT(45),
    Notes MEMO,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

CREATE INDEX IX_AuditLog_Timestamp ON AuditLog (Timestamp);
CREATE INDEX IX_AuditLog_User ON AuditLog (UserID);
```

### Step 3: Insert Sample Data

#### Insert Default Admin User:
```sql
INSERT INTO Users (Username, PasswordHash, FullName, Email, Role, Status)
VALUES ('admin', 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', 'System Administrator', 'admin@stockroom.local', 'Admin', 'Active');
```
*Note: This password hash represents "admin123"*

#### Insert Sample Categories:
```sql
INSERT INTO Categories (CategoryName, Description) VALUES 
('Electronics', 'Electronic devices and components'),
('Office Supplies', 'General office supplies and stationery'),
('Tools', 'Hand tools and equipment'),
('Furniture', 'Office and warehouse furniture'),
('Safety Equipment', 'Safety gear and equipment');
```

#### Insert Sample Suppliers:
```sql
INSERT INTO Suppliers (SupplierName, ContactPerson, Phone, Email, City, Country) VALUES 
('Tech Solutions Inc.', 'John Smith', '+1-555-0123', 'john@techsolutions.com', 'New York', 'USA'),
('Office Pro Supply', 'Mary Johnson', '+1-555-0456', 'mary@officepro.com', 'Chicago', 'USA'),
('Industrial Tools Co.', 'Bob Wilson', '+1-555-0789', 'bob@industrialtools.com', 'Detroit', 'USA');
```

#### Insert Sample Inventory Items:
```sql
INSERT INTO InventoryItems (ItemName, Description, SKU, CategoryID, SupplierID, Quantity, MinimumStock, ReorderLevel, CostPrice, SellingPrice, Location) VALUES 
('Wireless Mouse', 'Ergonomic wireless optical mouse', 'WM-001', 1, 1, 50, 10, 15, 25.00, 39.99, 'Shelf A1'),
('Keyboard', 'Full-size mechanical keyboard', 'KB-001', 1, 1, 30, 5, 10, 75.00, 129.99, 'Shelf A2'),
('Copy Paper', 'Letter size white copy paper (500 sheets)', 'CP-001', 2, 2, 100, 20, 30, 5.99, 9.99, 'Storage Room B'),
('Drill Set', '20-piece cordless drill set', 'DS-001', 3, 3, 15, 3, 5, 89.99, 149.99, 'Tool Cabinet');
```

---

## Connection String Configuration

Update your `DatabaseHelper.cs` connection string:

```csharp
string dbPath = Path.Combine(Application.StartupPath, "Database", "StockRoom.accdb");
connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};";
```

---

## Security Considerations

1. **Password Hashing**: All passwords are stored as SHA256 hashes with salt
2. **User Roles**: Implement role-based access control
3. **Audit Trail**: All changes are logged in the AuditLog table
4. **Database Security**: Secure the database file with appropriate permissions

---

## Backup Strategy

1. **Daily Backups**: Implement automated daily backups
2. **Before Updates**: Always backup before system updates
3. **Test Restores**: Regularly test backup restore procedures
4. **Off-site Storage**: Store backups in secure off-site location

---

## Performance Optimization

1. **Indexes**: Create indexes on frequently queried fields
2. **Compact & Repair**: Regularly compact and repair the database
3. **Connection Pooling**: Use connection pooling for better performance
4. **Query Optimization**: Optimize complex queries with proper WHERE clauses

---

This database structure provides a solid foundation for your inventory management system with full CRUD operations, user management, and audit trails.