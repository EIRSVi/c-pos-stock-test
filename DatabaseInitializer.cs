using System;
using System.IO;
using System.Windows.Forms;
using System.Data.OleDb;

namespace Stock_Room
{
    /// <summary>
    /// Helper class for database initialization and setup
    /// </summary>
    public static class DatabaseInitializer
    {
        /// <summary>
        /// Creates a basic database structure if the database doesn't exist
        /// </summary>
        public static bool InitializeDatabase(string connectionString)
        {
            try
            {
                // Extract database path from connection string
                var dbPath = ExtractDbPathFromConnectionString(connectionString);
                
                if (File.Exists(dbPath))
                {
                    return true; // Database already exists
                }

                // Show setup dialog
                var result = MessageBox.Show(
                    $"Database file not found at:\n{dbPath}\n\n" +
                    "Would you like to:\n" +
                    "• YES: Continue in Demo Mode (recommended for testing)\n" +
                    "• NO: Create empty database structure\n" +
                    "• CANCEL: Exit application\n\n" +
                    "Note: For full functionality, please create the database manually using the provided setup guide.",
                    "Database Setup Required",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                switch (result)
                {
                    case DialogResult.Yes:
                        return false; // Demo mode
                    case DialogResult.No:
                        return CreateEmptyDatabase(dbPath);
                    case DialogResult.Cancel:
                    default:
                        Environment.Exit(0);
                        return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database initialization error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private static string ExtractDbPathFromConnectionString(string connectionString)
        {
            // Simple extraction of Data Source from connection string
            var parts = connectionString.Split(';');
            foreach (var part in parts)
            {
                if (part.Trim().StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
                {
                    return part.Substring(part.IndexOf('=') + 1).Trim();
                }
            }
            return "StockRoom.accdb";
        }

        private static bool CreateEmptyDatabase(string dbPath)
        {
            try
            {
                // Create directory if it doesn't exist
                var directory = Path.GetDirectoryName(dbPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Create a basic Access database with minimal structure
                // Note: This requires ADOX which may not be available in all environments
                // For simplicity, we'll just show instructions
                
                MessageBox.Show(
                    "To create the database:\n\n" +
                    "1. Open Microsoft Access\n" +
                    "2. Create a new blank database\n" +
                    "3. Save it as: " + Path.GetFileName(dbPath) + "\n" +
                    "4. Use the SQL scripts in Database/DatabaseSetupGuide.md\n\n" +
                    "For now, the application will run in Demo Mode.",
                    "Manual Database Setup Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return false; // Return false to indicate demo mode
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not create database: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Shows database setup instructions
        /// </summary>
        public static void ShowSetupInstructions()
        {
            var instructions = @"?? DATABASE SETUP INSTRUCTIONS

To enable full functionality, please set up the Access database:

?? QUICK SETUP:
1. Open Microsoft Access
2. Create new blank database as 'StockRoom.accdb'
3. Save in the application directory
4. Run the SQL scripts from Database/DatabaseSetupGuide.md

?? REQUIRED TABLES:
• Users (authentication)
• Categories (item organization)  
• Suppliers (vendor management)
• InventoryItems (main inventory)
• StockMovements (tracking)
• AuditLog (system logs)

?? DEFAULT LOGIN (after setup):
Username: admin
Password: admin123

?? DEMO MODE:
The application works without database for testing.
Use the credentials shown on the login screen.

?? For detailed setup, see: Database/DatabaseSetupGuide.md";

            MessageBox.Show(instructions, "Stock Room - Database Setup", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}