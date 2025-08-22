using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace Stock_Room
{
    public class DatabaseHelper
    {
        private readonly string connectionString;
        private bool databaseAvailable = false;

        public DatabaseHelper()
        {
            // Connection string for MS Access database
            connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=V:\LoginDB.accdb;";
            databaseAvailable = TestConnection();
        }

        public bool ValidateUser(string username, string password)
        {
            // If database is available, use database authentication
            if (databaseAvailable)
            {
                return ValidateUserFromDatabase(username, password);
            }
            else
            {
                // Fallback to hardcoded demo credentials for testing
                return ValidateUserDemo(username, password);
            }
        }

        private bool ValidateUserFromDatabase(string username, string password)
        {
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    
                    // First check if Users table exists
                    if (!TableExists(connection, "Users"))
                    {
                        MessageBox.Show("Users table not found in database. Creating table...", "Database Setup", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CreateUsersTable(connection);
                    }
                    
                    string query = "SELECT COUNT(*) FROM Users WHERE Username = ? AND Password = ?";
                    
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);
                        
                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database error: {ex.Message}\n\nSwitching to demo mode.", "Database Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                databaseAvailable = false;
                return ValidateUserDemo(username, password);
            }
        }

        private bool TableExists(OleDbConnection connection, string tableName)
        {
            try
            {
                DataTable tables = connection.GetSchema("Tables");
                foreach (DataRow row in tables.Rows)
                {
                    if (row["TABLE_NAME"].ToString().Equals(tableName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private void CreateUsersTable(OleDbConnection connection)
        {
            try
            {
                string createTableQuery = @"
                    CREATE TABLE Users (
                        ID COUNTER PRIMARY KEY,
                        Username TEXT(50) NOT NULL,
                        Password TEXT(100) NOT NULL,
                        FullName TEXT(100) NOT NULL,
                        UserRole TEXT(50) NOT NULL
                    )";

                using (OleDbCommand command = new OleDbCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                // Insert default users
                InsertDefaultUsers(connection);
                
                MessageBox.Show("Users table created successfully with default users!", "Database Setup Complete", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating Users table: {ex.Message}", "Database Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                databaseAvailable = false;
            }
        }

        private void InsertDefaultUsers(OleDbConnection connection)
        {
            try
            {
                string[] insertQueries = {
                    "INSERT INTO Users (Username, Password, FullName, UserRole) VALUES ('admin', 'admin123', 'System Administrator', 'Admin')",
                    "INSERT INTO Users (Username, Password, FullName, UserRole) VALUES ('user1', 'user123', 'John Doe', 'User')",
                    "INSERT INTO Users (Username, Password, FullName, UserRole) VALUES ('manager', 'manager123', 'Jane Smith', 'Manager')"
                };

                foreach (string query in insertQueries)
                {
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting default users: {ex.Message}", "Database Warning", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private bool ValidateUserDemo(string username, string password)
        {
            // Demo credentials for testing without database
            var demoUsers = new[]
            {
                new { Username = "admin", Password = "admin123" },
                new { Username = "user1", Password = "user123" },
                new { Username = "manager", Password = "manager123" }
            };

            foreach (var user in demoUsers)
            {
                if (user.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && 
                    user.Password == password)
                {
                    return true;
                }
            }

            return false;
        }

        public UserInfo GetUserInfo(string username)
        {
            // If database is available, get from database
            if (databaseAvailable)
            {
                return GetUserInfoFromDatabase(username);
            }
            else
            {
                // Fallback to demo user info
                return GetDemoUserInfo(username);
            }
        }

        private UserInfo GetUserInfoFromDatabase(string username)
        {
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT ID, Username, FullName, UserRole FROM Users WHERE Username = ?";
                    
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        
                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new UserInfo
                                {
                                    ID = reader.GetInt32("ID"),
                                    Username = reader.GetString("Username"),
                                    FullName = reader.GetString("FullName"),
                                    UserRole = reader.GetString("UserRole")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting user info: {ex.Message}\n\nUsing demo user info.", "Database Warning", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                databaseAvailable = false;
                return GetDemoUserInfo(username);
            }
            
            return GetDemoUserInfo(username);
        }

        private UserInfo GetDemoUserInfo(string username)
        {
            // Demo user information for testing
            switch (username.ToLower())
            {
                case "admin":
                    return new UserInfo
                    {
                        ID = 1,
                        Username = "admin",
                        FullName = "System Administrator",
                        UserRole = "Admin"
                    };
                case "user1":
                    return new UserInfo
                    {
                        ID = 2,
                        Username = "user1",
                        FullName = "John Doe",
                        UserRole = "User"
                    };
                case "manager":
                    return new UserInfo
                    {
                        ID = 3,
                        Username = "manager",
                        FullName = "Jane Smith",
                        UserRole = "Manager"
                    };
                default:
                    return new UserInfo
                    {
                        ID = 0,
                        Username = username,
                        FullName = "Demo User",
                        UserRole = "User"
                    };
            }
        }

        public bool TestConnection()
        {
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    databaseAvailable = true;
                    return true;
                }
            }
            catch
            {
                databaseAvailable = false;
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
                return "Connected to database at V:\\LoginDB.accdb";
            }
            else
            {
                return "Demo Mode: Database not available. Using hardcoded credentials.";
            }
        }

        public void RefreshConnection()
        {
            databaseAvailable = TestConnection();
        }
    }

    public class UserInfo
    {
        public int ID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
    }
}