# Stock Room Management System

A Windows Forms application built with C# and .NET 8, featuring a login system with MS Access database integration and a modern dashboard interface.

## Features

- **Secure Login System**: Username/password authentication with MS Access database
- **Modern UI**: Clean and professional interface with logo area
- **Dashboard**: Main dashboard with sidebar navigation
- **User Management**: Role-based access system
- **Responsive Design**: Adapts to different screen sizes

## System Requirements

- Windows 10 or later
- .NET 8.0 Runtime
- Microsoft Access Database Engine (ACE.OLEDB.12.0)
- Visual Studio Community 2022 (for development)

## Database Setup

### Method 1: Using Microsoft Access
1. Open Microsoft Access
2. Create a new blank database
3. Save it as `LoginDB.accdb` at location `V:\LoginDB.accdb`
4. Create a new table named "Users" with the following fields:
   - ID (AutoNumber, Primary Key)
   - Username (Short Text, 50 characters, Required)
   - Password (Short Text, 100 characters, Required)
   - FullName (Short Text, 100 characters, Required)
   - UserRole (Short Text, 50 characters, Required)

### Method 2: Using SQL in Access
1. Create the database file at `V:\LoginDB.accdb`
2. Open Access and go to Create > Query Design
3. Close the Show Table dialog
4. Go to Design > SQL View
5. Paste and run the following SQL:

```sql
CREATE TABLE Users (
    ID COUNTER PRIMARY KEY,
    Username TEXT(50) NOT NULL,
    Password TEXT(100) NOT NULL,
    FullName TEXT(100) NOT NULL,
    UserRole TEXT(50) NOT NULL
);
```

6. Insert sample data:
```sql
INSERT INTO Users (Username, Password, FullName, UserRole)
VALUES ('admin', 'admin123', 'Administrator', 'Admin');

INSERT INTO Users (Username, Password, FullName, UserRole)
VALUES ('user1', 'user123', 'John Doe', 'User');

INSERT INTO Users (Username, Password, FullName, UserRole)
VALUES ('manager', 'manager123', 'Jane Smith', 'Manager');
```

## Installation & Running

### For Development:
1. Open Visual Studio Community 2022
2. Open the project file `Stock Room.csproj`
3. Restore NuGet packages (should happen automatically)
4. Build the solution (Ctrl+Shift+B)
5. Run the application (F5)

### For Distribution:
1. Build the project in Release mode
2. Publish the application:
   - Right-click project > Publish
   - Choose folder publish
   - Select target location
   - Click Publish

## Usage

### Login
1. Launch the application
2. Enter your username and password
3. Click LOGIN or press Enter
4. Use ESC to exit the application

### Test Credentials
- **Administrator**: admin / admin123
- **Regular User**: user1 / user123
- **Manager**: manager / manager123

### Dashboard Navigation
- **Dashboard**: Overview with summary cards
- **Inventory**: Stock management (placeholder)
- **Reports**: Generate various reports (placeholder)
- **Settings**: System configuration (placeholder)
- **Logout**: Return to login screen

## Project Structure

```
Stock Room/
??? Stock Room.csproj          # Project file with dependencies
??? Program.cs                 # Application entry point
??? DatabaseHelper.cs          # MS Access database operations
??? LoginForm.cs              # Login interface
??? DashboardForm.cs          # Main dashboard interface
??? DatabaseSetup.sql         # Database creation script
??? README.md                 # This file
```

## Key Classes

### DatabaseHelper
- Handles MS Access database connections
- User authentication validation
- User information retrieval
- Connection testing

### LoginForm
- Login interface with logo area
- Username/password input
- Authentication handling
- Error display

### DashboardForm
- Main application interface
- Sidebar navigation
- Content area with modules
- User session management

### UserInfo
- Data model for user information
- Contains ID, Username, FullName, UserRole

## Dependencies

- **System.Data.OleDb**: For MS Access database connectivity
- **System.Windows.Forms**: For Windows Forms UI
- **.NET 8.0**: Target framework

## Security Considerations

?? **Important**: This is a basic implementation for demonstration purposes. For production use, consider:

- Encrypting/hashing passwords instead of plain text storage
- Using SQL parameters to prevent SQL injection (already implemented)
- Adding input validation and sanitization
- Implementing session management
- Adding audit logging
- Using more secure database solutions

## Troubleshooting

### Database Connection Issues
- Ensure the database file exists at `V:\LoginDB.accdb`
- Verify Microsoft Access Database Engine is installed
- Check file permissions on the database file

### Build Errors
- Restore NuGet packages: Tools > NuGet Package Manager > Restore
- Clean and rebuild solution: Build > Clean Solution, then Build > Rebuild Solution

### Runtime Errors
- Check if .NET 8.0 runtime is installed
- Verify all required dependencies are available

## Future Enhancements

- Implement actual inventory management functionality
- Add reporting capabilities
- Implement user management interface
- Add data validation and error handling
- Implement backup/restore functionality
- Add configuration management
- Implement audit trail

## License

This project is created for educational and demonstration purposes.

## Support

For issues and questions, please refer to the code comments and documentation within the source files.