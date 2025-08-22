using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Stock_Room.Models;
using System.Windows.Forms;

namespace Stock_Room
{
    /// <summary>
    /// Enhanced database helper with comprehensive CRUD operations and connection management
    /// </summary>
    public class DatabaseHelper : IDisposable
    {
        private readonly string connectionString;
        private OleDbConnection? connection;
        private bool databaseAvailable = false;

        public DatabaseHelper()
        {
            // Try multiple possible database locations
            string[] possiblePaths = {
                Path.Combine(Application.StartupPath, "StockRoom.accdb"),
                Path.Combine(Application.StartupPath, "Database", "StockRoom.accdb"),
                "StockRoom.accdb"
            };

            string dbPath = "";
            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    dbPath = path;
                    break;
                }
            }

            if (string.IsNullOrEmpty(dbPath))
            {
                // Use the preferred path for error messages
                dbPath = possiblePaths[0];
            }

            connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};";
            databaseAvailable = TestConnection();
        }

        public DatabaseHelper(string customConnectionString)
        {
            connectionString = customConnectionString;
        }

        /// <summary>
        /// Ensures database connection is open
        /// </summary>
        private async Task EnsureConnectionAsync()
        {
            try
            {
                if (connection == null)
                {
                    connection = new OleDbConnection(connectionString);
                }

                if (connection.State != ConnectionState.Open)
                {
                    // Use proper async connection opening
                    await connection.OpenAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the error and rethrow with more context
                System.Diagnostics.Debug.WriteLine($"Database connection failed: {ex.Message}");
                throw new DatabaseException($"Failed to open database connection: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Executes a query and returns the result as DataTable
        /// </summary>
        public async Task<DataTable> ExecuteQueryAsync(string query, params OleDbParameter[] parameters)
        {
            try
            {
                await EnsureConnectionAsync();
                
                using var command = new OleDbCommand(query, connection);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                using var adapter = new OleDbDataAdapter(command);
                var dataTable = new DataTable();
                
                await Task.Run(() => adapter.Fill(dataTable));
                return dataTable;
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Query execution failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Executes a non-query command and returns affected rows count
        /// </summary>
        public async Task<int> ExecuteNonQueryAsync(string query, params OleDbParameter[] parameters)
        {
            try
            {
                await EnsureConnectionAsync();
                
                using var command = new OleDbCommand(query, connection);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                return await Task.Run(() => command.ExecuteNonQuery());
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Command execution failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Executes a scalar query and returns the first column of the first row
        /// </summary>
        public async Task<T> ExecuteScalarAsync<T>(string query, params OleDbParameter[] parameters)
        {
            try
            {
                await EnsureConnectionAsync();
                
                using var command = new OleDbCommand(query, connection);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                var result = await Task.Run(() => command.ExecuteScalar());
                
                if (result == null || result == DBNull.Value)
                    return default(T)!;

                return (T)Convert.ChangeType(result, typeof(T));
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Scalar execution failed: {ex.Message}", ex);
            }
        }

        #region User Management

        /// <summary>
        /// Validates user login credentials
        /// </summary>
        public async Task<UserInfo?> ValidateUserAsync(string username, string password)
        {
            try
            {
                string hashedPassword = HashPassword(password);
                string query = @"
                    SELECT UserID, Username, FullName, Email, Phone, Role, Status, LastLogin 
                    FROM Users 
                    WHERE Username = ? AND PasswordHash = ? AND Status = 'Active'";

                var parameters = new OleDbParameter[]
                {
                    new OleDbParameter("@username", username),
                    new OleDbParameter("@password", hashedPassword)
                };

                var dataTable = await ExecuteQueryAsync(query, parameters);
                
                if (dataTable.Rows.Count == 1)
                {
                    var row = dataTable.Rows[0];
                    var userInfo = new UserInfo
                    {
                        UserID = Convert.ToInt32(row["UserID"]),
                        Username = row["Username"].ToString() ?? string.Empty,
                        FullName = row["FullName"].ToString() ?? string.Empty,
                        Email = row["Email"].ToString() ?? string.Empty,
                        Phone = row["Phone"].ToString() ?? string.Empty,
                        UserRole = row["Role"].ToString() ?? string.Empty,
                        Status = row["Status"].ToString() ?? string.Empty,
                        LastLogin = row["LastLogin"] as DateTime?
                    };

                    // Update last login time
                    await UpdateLastLoginAsync(userInfo.UserID);
                    
                    return userInfo;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"User validation failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Updates user's last login timestamp
        /// </summary>
        private async Task UpdateLastLoginAsync(int userId)
        {
            string query = "UPDATE Users SET LastLogin = ? WHERE UserID = ?";
            var parameters = new OleDbParameter[]
            {
                new OleDbParameter("@lastLogin", DateTime.Now),
                new OleDbParameter("@userId", userId)
            };

            await ExecuteNonQueryAsync(query, parameters);
        }

        /// <summary>
        /// Creates a new user account
        /// </summary>
        public async Task<int> CreateUserAsync(User user)
        {
            try
            {
                string query = @"
                    INSERT INTO Users (Username, PasswordHash, FullName, Email, Phone, Role, CreatedDate, Status, Notes)
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)";

                var parameters = new OleDbParameter[]
                {
                    new OleDbParameter("@username", user.Username),
                    new OleDbParameter("@passwordHash", user.PasswordHash),
                    new OleDbParameter("@fullName", user.FullName),
                    new OleDbParameter("@email", user.Email),
                    new OleDbParameter("@phone", user.Phone),
                    new OleDbParameter("@role", user.Role),
                    new OleDbParameter("@createdDate", user.CreatedDate),
                    new OleDbParameter("@status", user.Status),
                    new OleDbParameter("@notes", user.Notes)
                };

                await ExecuteNonQueryAsync(query, parameters);
                
                // Get the new user ID
                string getIdQuery = "SELECT @@IDENTITY";
                return await ExecuteScalarAsync<int>(getIdQuery);
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"User creation failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets all users for management
        /// </summary>
        public async Task<List<User>> GetUsersAsync()
        {
            try
            {
                string query = @"
                    SELECT UserID, Username, FullName, Email, Phone, Role, CreatedDate, LastLogin, Status, Notes
                    FROM Users 
                    ORDER BY FullName";

                var dataTable = await ExecuteQueryAsync(query);
                return MapToUsers(dataTable);
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to retrieve users: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Updates an existing user
        /// </summary>
        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                string query = @"
                    UPDATE Users SET
                    Username = ?, FullName = ?, Email = ?, Phone = ?, Role = ?, Status = ?, Notes = ?
                    WHERE UserID = ?";

                var parameters = new OleDbParameter[]
                {
                    new OleDbParameter("@username", user.Username),
                    new OleDbParameter("@fullName", user.FullName),
                    new OleDbParameter("@email", user.Email),
                    new OleDbParameter("@phone", user.Phone),
                    new OleDbParameter("@role", user.Role),
                    new OleDbParameter("@status", user.Status),
                    new OleDbParameter("@notes", user.Notes),
                    new OleDbParameter("@userId", user.UserID)
                };

                int affectedRows = await ExecuteNonQueryAsync(query, parameters);
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to update user: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates a new user with hashed password
        /// </summary>
        public async Task<int> CreateUserWithPasswordAsync(string username, string password, string fullName, string email, string phone, string role)
        {
            try
            {
                var user = new User
                {
                    Username = username,
                    PasswordHash = HashPassword(password),
                    FullName = fullName,
                    Email = email,
                    Phone = phone,
                    Role = role,
                    CreatedDate = DateTime.Now,
                    Status = "Active",
                    Notes = ""
                };

                return await CreateUserAsync(user);
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to create user: {ex.Message}", ex);
            }
        }

        #endregion

        #region Inventory Management

        /// <summary>
        /// Gets all inventory items with optional filtering
        /// </summary>
        public async Task<List<InventoryItem>> GetInventoryItemsAsync(InventorySearchParams? searchParams = null)
        {
            try
            {
                var whereClause = new StringBuilder();
                var parameters = new List<OleDbParameter>();

                string baseQuery = @"
                    SELECT i.*, c.CategoryName, s.SupplierName 
                    FROM (InventoryItems i 
                    LEFT JOIN Categories c ON i.CategoryID = c.CategoryID) 
                    LEFT JOIN Suppliers s ON i.SupplierID = s.SupplierID";

                if (searchParams != null)
                {
                    var conditions = new List<string>();

                    if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
                    {
                        conditions.Add("(i.ItemName LIKE ? OR i.Description LIKE ? OR i.SKU LIKE ?)");
                        var searchTerm = $"%{searchParams.SearchTerm}%";
                        parameters.Add(new OleDbParameter("@search1", searchTerm));
                        parameters.Add(new OleDbParameter("@search2", searchTerm));
                        parameters.Add(new OleDbParameter("@search3", searchTerm));
                    }

                    if (searchParams.CategoryID.HasValue)
                    {
                        conditions.Add("i.CategoryID = ?");
                        parameters.Add(new OleDbParameter("@categoryId", searchParams.CategoryID.Value));
                    }

                    if (searchParams.SupplierID.HasValue)
                    {
                        conditions.Add("i.SupplierID = ?");
                        parameters.Add(new OleDbParameter("@supplierId", searchParams.SupplierID.Value));
                    }

                    if (!string.IsNullOrWhiteSpace(searchParams.Status))
                    {
                        conditions.Add("i.Status = ?");
                        parameters.Add(new OleDbParameter("@status", searchParams.Status));
                    }

                    if (searchParams.LowStockOnly == true)
                    {
                        conditions.Add("i.Quantity <= i.MinimumStock");
                    }

                    if (conditions.Count > 0)
                    {
                        whereClause.Append(" WHERE ").Append(string.Join(" AND ", conditions));
                    }
                }

                string orderClause = $" ORDER BY i.{searchParams?.SortBy ?? "ItemName"} {searchParams?.SortOrder ?? "ASC"}";
                string query = baseQuery + whereClause.ToString() + orderClause;

                var dataTable = await ExecuteQueryAsync(query, parameters.ToArray());
                return MapToInventoryItems(dataTable);
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to retrieve inventory items: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets a single inventory item by ID
        /// </summary>
        public async Task<InventoryItem?> GetInventoryItemAsync(int itemId)
        {
            try
            {
                string query = @"
                    SELECT i.*, c.CategoryName, s.SupplierName 
                    FROM (InventoryItems i 
                    LEFT JOIN Categories c ON i.CategoryID = c.CategoryID) 
                    LEFT JOIN Suppliers s ON i.SupplierID = s.SupplierID
                    WHERE i.ItemID = ?";

                var parameters = new OleDbParameter[] { new OleDbParameter("@itemId", itemId) };
                var dataTable = await ExecuteQueryAsync(query, parameters);
                
                var items = MapToInventoryItems(dataTable);
                return items.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to retrieve inventory item: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates a new inventory item
        /// </summary>
        public async Task<int> CreateInventoryItemAsync(InventoryItem item)
        {
            try
            {
                string query = @"
                    INSERT INTO InventoryItems 
                    (ItemName, Description, SKU, Barcode, CategoryID, SupplierID, Quantity, MinimumStock, 
                     ReorderLevel, Unit, CostPrice, SellingPrice, CreatedDate, Status, Location, Notes)
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
                var parameters = new OleDbParameter[]
                {
                    new OleDbParameter("@itemName", item.ItemName),
                    new OleDbParameter("@description", item.Description),
                    new OleDbParameter("@sku", item.SKU),
                    new OleDbParameter("@barcode", item.Barcode),
                    new OleDbParameter("@categoryId", item.CategoryID),
                    new OleDbParameter("@supplierId", item.SupplierID),
                    new OleDbParameter("@quantity", item.Quantity),
                    new OleDbParameter("@minimumStock", item.MinimumStock),
                    new OleDbParameter("@reorderLevel", item.ReorderLevel),
                    new OleDbParameter("@unit", item.Unit),
                    new OleDbParameter("@costPrice", item.CostPrice),
                    new OleDbParameter("@sellingPrice", item.SellingPrice),
                    new OleDbParameter("@createdDate", item.CreatedDate),
                    new OleDbParameter("@status", item.Status),
                    new OleDbParameter("@location", item.Location),
                    new OleDbParameter("@notes", item.Notes)
                };

                await ExecuteNonQueryAsync(query, parameters);
                
                string getIdQuery = "SELECT @@IDENTITY";
                return await ExecuteScalarAsync<int>(getIdQuery);
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to create inventory item: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Updates an existing inventory item
        /// </summary>
        public async Task<bool> UpdateInventoryItemAsync(InventoryItem item)
        {
            try
            {
                string query = @"
                    UPDATE InventoryItems SET
                    ItemName = ?, Description = ?, SKU = ?, Barcode = ?, CategoryID = ?, SupplierID = ?,
                    Quantity = ?, MinimumStock = ?, ReorderLevel = ?, Unit = ?, CostPrice = ?, SellingPrice = ?,
                    LastUpdated = ?, Status = ?, Location = ?, Notes = ?
                    WHERE ItemID = ?";

                var parameters = new OleDbParameter[]
                {
                    new OleDbParameter("@itemName", item.ItemName),
                    new OleDbParameter("@description", item.Description),
                    new OleDbParameter("@sku", item.SKU),
                    new OleDbParameter("@barcode", item.Barcode),
                    new OleDbParameter("@categoryId", item.CategoryID),
                    new OleDbParameter("@supplierId", item.SupplierID),
                    new OleDbParameter("@quantity", item.Quantity),
                    new OleDbParameter("@minimumStock", item.MinimumStock),
                    new OleDbParameter("@reorderLevel", item.ReorderLevel),
                    new OleDbParameter("@unit", item.Unit),
                    new OleDbParameter("@costPrice", item.CostPrice),
                    new OleDbParameter("@sellingPrice", item.SellingPrice),
                    new OleDbParameter("@lastUpdated", DateTime.Now),
                    new OleDbParameter("@status", item.Status),
                    new OleDbParameter("@location", item.Location),
                    new OleDbParameter("@notes", item.Notes),
                    new OleDbParameter("@itemId", item.ItemID)
                };

                int affectedRows = await ExecuteNonQueryAsync(query, parameters);
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to update inventory item: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Deletes an inventory item (soft delete by setting status to Inactive)
        /// </summary>
        public async Task<bool> DeleteInventoryItemAsync(int itemId)
        {
            try
            {
                string query = "UPDATE InventoryItems SET Status = 'Inactive', LastUpdated = ? WHERE ItemID = ?";
                var parameters = new OleDbParameter[]
                {
                    new OleDbParameter("@lastUpdated", DateTime.Now),
                    new OleDbParameter("@itemId", itemId)
                };

                int affectedRows = await ExecuteNonQueryAsync(query, parameters);
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to delete inventory item: {ex.Message}", ex);
            }
        }

        #endregion

        #region Categories Management

        /// <summary>
        /// Gets all categories
        /// </summary>
        public async Task<List<Category>> GetCategoriesAsync()
        {
            try
            {
                string query = @"
                    SELECT c.*, COUNT(i.ItemID) as ItemCount 
                    FROM Categories c 
                    LEFT JOIN InventoryItems i ON c.CategoryID = i.CategoryID AND i.Status = 'Active'
                    WHERE c.Status = 'Active'
                    GROUP BY c.CategoryID, c.CategoryName, c.Description, c.CreatedDate, c.Status
                    ORDER BY c.CategoryName";

                var dataTable = await ExecuteQueryAsync(query);
                return MapToCategories(dataTable);
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to retrieve categories: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates a new category
        /// </summary>
        public async Task<int> CreateCategoryAsync(Category category)
        {
            try
            {
                string query = @"
                    INSERT INTO Categories (CategoryName, Description, CreatedDate, Status)
                    VALUES (?, ?, ?, ?)";

                var parameters = new OleDbParameter[]
                {
                    new OleDbParameter("@categoryName", category.CategoryName),
                    new OleDbParameter("@description", category.Description),
                    new OleDbParameter("@createdDate", category.CreatedDate),
                    new OleDbParameter("@status", category.Status)
                };

                await ExecuteNonQueryAsync(query, parameters);
                
                string getIdQuery = "SELECT @@IDENTITY";
                return await ExecuteScalarAsync<int>(getIdQuery);
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to create category: {ex.Message}", ex);
            }
        }

        #endregion

        #region Suppliers Management

        /// <summary>
        /// Gets all suppliers
        /// </summary>
        public async Task<List<Supplier>> GetSuppliersAsync()
        {
            try
            {
                string query = @"
                    SELECT s.*, COUNT(i.ItemID) as ItemCount 
                    FROM Suppliers s 
                    LEFT JOIN InventoryItems i ON s.SupplierID = i.SupplierID AND i.Status = 'Active'
                    WHERE s.Status = 'Active'
                    GROUP BY s.SupplierID, s.SupplierName, s.ContactPerson, s.Phone, s.Email, s.Address, 
                             s.City, s.PostalCode, s.Country, s.CreatedDate, s.Status, s.Notes
                    ORDER BY s.SupplierName";

                var dataTable = await ExecuteQueryAsync(query);
                return MapToSuppliers(dataTable);
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to retrieve suppliers: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates a new supplier
        /// </summary>
        public async Task<int> CreateSupplierAsync(Supplier supplier)
        {
            try
            {
                string query = @"
                    INSERT INTO Suppliers 
                    (SupplierName, ContactPerson, Phone, Email, Address, City, PostalCode, Country, CreatedDate, Status, Notes)
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                var parameters = new OleDbParameter[]
                {
                    new OleDbParameter("@supplierName", supplier.SupplierName),
                    new OleDbParameter("@contactPerson", supplier.ContactPerson),
                    new OleDbParameter("@phone", supplier.Phone),
                    new OleDbParameter("@email", supplier.Email),
                    new OleDbParameter("@address", supplier.Address),
                    new OleDbParameter("@city", supplier.City),
                    new OleDbParameter("@postalCode", supplier.PostalCode),
                    new OleDbParameter("@country", supplier.Country),
                    new OleDbParameter("@createdDate", supplier.CreatedDate),
                    new OleDbParameter("@status", supplier.Status),
                    new OleDbParameter("@notes", supplier.Notes)
                };

                await ExecuteNonQueryAsync(query, parameters);
                
                string getIdQuery = "SELECT @@IDENTITY";
                return await ExecuteScalarAsync<int>(getIdQuery);
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to create supplier: {ex.Message}", ex);
            }
        }

        #endregion

        #region Dashboard Statistics

        /// <summary>
        /// Gets dashboard statistics
        /// </summary>
        public async Task<DashboardStats> GetDashboardStatsAsync()
        {
            try
            {
                var stats = new DashboardStats();

                // Total items
                stats.TotalItems = await ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM InventoryItems WHERE Status = 'Active'");

                // Low stock items
                stats.LowStockItems = await ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM InventoryItems WHERE Status = 'Active' AND Quantity <= MinimumStock");

                // Out of stock items
                stats.OutOfStockItems = await ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM InventoryItems WHERE Status = 'Active' AND Quantity = 0");

                // Total categories
                stats.TotalCategories = await ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM Categories WHERE Status = 'Active'");

                // Total suppliers
                stats.TotalSuppliers = await ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM Suppliers WHERE Status = 'Active'");

                // Active users
                stats.ActiveUsers = await ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM Users WHERE Status = 'Active'");

                // Total inventory value
                stats.TotalInventoryValue = await ExecuteScalarAsync<decimal>(
                    "SELECT SUM(Quantity * CostPrice) FROM InventoryItems WHERE Status = 'Active'");

                // Recent movements (last 30 days)
                stats.RecentMovements = await ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM StockMovements WHERE MovementDate >= ?",
                    new OleDbParameter("@date", DateTime.Now.AddDays(-30)));

                stats.LastUpdated = DateTime.Now;
                return stats;
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Failed to retrieve dashboard statistics: {ex.Message}", ex);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Maps DataTable to InventoryItem list
        /// </summary>
        private List<InventoryItem> MapToInventoryItems(DataTable dataTable)
        {
            var items = new List<InventoryItem>();
            
            foreach (DataRow row in dataTable.Rows)
            {
                items.Add(new InventoryItem
                {
                    ItemID = Convert.ToInt32(row["ItemID"]),
                    ItemName = row["ItemName"].ToString() ?? string.Empty,
                    Description = row["Description"].ToString() ?? string.Empty,
                    SKU = row["SKU"].ToString() ?? string.Empty,
                    Barcode = row["Barcode"].ToString() ?? string.Empty,
                    CategoryID = Convert.ToInt32(row["CategoryID"]),
                    SupplierID = Convert.ToInt32(row["SupplierID"]),
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    MinimumStock = Convert.ToInt32(row["MinimumStock"]),
                    ReorderLevel = Convert.ToInt32(row["ReorderLevel"]),
                    Unit = row["Unit"].ToString() ?? "pcs",
                    CostPrice = Convert.ToDecimal(row["CostPrice"]),
                    SellingPrice = Convert.ToDecimal(row["SellingPrice"]),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    LastUpdated = row["LastUpdated"] as DateTime?,
                    LastRestocked = row["LastRestocked"] as DateTime?,
                    Status = row["Status"].ToString() ?? "Active",
                    Location = row["Location"].ToString() ?? string.Empty,
                    Notes = row["Notes"].ToString() ?? string.Empty,
                    CategoryName = row["CategoryName"].ToString(),
                    SupplierName = row["SupplierName"].ToString()
                });
            }
            
            return items;
        }

        /// <summary>
        /// Maps DataTable to Category list
        /// </summary>
        private List<Category> MapToCategories(DataTable dataTable)
        {
            var categories = new List<Category>();
            
            foreach (DataRow row in dataTable.Rows)
            {
                categories.Add(new Category
                {
                    CategoryID = Convert.ToInt32(row["CategoryID"]),
                    CategoryName = row["CategoryName"].ToString() ?? string.Empty,
                    Description = row["Description"].ToString() ?? string.Empty,
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    Status = row["Status"].ToString() ?? "Active",
                    ItemCount = Convert.ToInt32(row["ItemCount"])
                });
            }
            
            return categories;
        }

        /// <summary>
        /// Maps DataTable to Supplier list
        /// </summary>
        private List<Supplier> MapToSuppliers(DataTable dataTable)
        {
            var suppliers = new List<Supplier>();
            
            foreach (DataRow row in dataTable.Rows)
            {
                suppliers.Add(new Supplier
                {
                    SupplierID = Convert.ToInt32(row["SupplierID"]),
                    SupplierName = row["SupplierName"].ToString() ?? string.Empty,
                    ContactPerson = row["ContactPerson"].ToString() ?? string.Empty,
                    Phone = row["Phone"].ToString() ?? string.Empty,
                    Email = row["Email"].ToString() ?? string.Empty,
                    Address = row["Address"].ToString() ?? string.Empty,
                    City = row["City"].ToString() ?? string.Empty,
                    PostalCode = row["PostalCode"].ToString() ?? string.Empty,
                    Country = row["Country"].ToString() ?? string.Empty,
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    Status = row["Status"].ToString() ?? "Active",
                    Notes = row["Notes"].ToString() ?? string.Empty,
                    ItemCount = Convert.ToInt32(row["ItemCount"])
                });
            }
            
            return suppliers;
        }

        /// <summary>
        /// Maps DataTable to User list
        /// </summary>
        private List<User> MapToUsers(DataTable dataTable)
        {
            var users = new List<User>();
            
            foreach (DataRow row in dataTable.Rows)
            {
                users.Add(new User
                {
                    UserID = Convert.ToInt32(row["UserID"]),
                    Username = row["Username"].ToString() ?? string.Empty,
                    FullName = row["FullName"].ToString() ?? string.Empty,
                    Email = row["Email"].ToString() ?? string.Empty,
                    Phone = row["Phone"].ToString() ?? string.Empty,
                    Role = row["Role"].ToString() ?? "User",
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    LastLogin = row["LastLogin"] as DateTime?,
                    Status = row["Status"].ToString() ?? "Active",
                    Notes = row["Notes"].ToString() ?? string.Empty
                });
            }
            
            return users;
        }

        /// <summary>
        /// Hashes password using SHA256
        /// </summary>
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "StockRoom_Salt"));
            return Convert.ToBase64String(hashedBytes);
        }

        #endregion

        public void Dispose()
        {
            connection?.Close();
            connection?.Dispose();
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using (var testConnection = new OleDbConnection(connectionString))
                {
                    await testConnection.OpenAsync();
                    databaseAvailable = true;
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database test connection failed: {ex.Message}");
                // If database file doesn't exist or can't connect, try to initialize
                if (!databaseAvailable)
                {
                    databaseAvailable = DatabaseInitializer.InitializeDatabase(connectionString);
                }
                return false;
            }
        }

        public bool TestConnection()
        {
            try
            {
                using (var testConnection = new OleDbConnection(connectionString))
                {
                    testConnection.Open();
                    databaseAvailable = true;
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database test connection failed: {ex.Message}");
                // If database file doesn't exist or can't connect, try to initialize
                if (!databaseAvailable)
                {
                    databaseAvailable = DatabaseInitializer.InitializeDatabase(connectionString);
                }
                return false;
            }
        }

        public bool IsDatabaseAvailable()
        {
            return databaseAvailable;
        }

        public string GetConnectionStatus()
        {
            if (databaseAvailable)
            {
                return "? Connected to StockRoom.accdb database";
            }
            else
            {
                return "?? Demo Mode: Database file not found. Create StockRoom.accdb or use demo credentials.";
            }
        }

        public void RefreshConnection()
        {
            databaseAvailable = TestConnection();
        }
    }

    /// <summary>
    /// Custom exception for database operations
    /// </summary>
    public class DatabaseException : Exception
    {
        public DatabaseException(string message) : base(message) { }
        public DatabaseException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class UserInfo
    {
        public int UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? LastLogin { get; set; } = null;
    }
}