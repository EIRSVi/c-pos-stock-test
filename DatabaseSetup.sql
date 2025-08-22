/*
DATABASE SETUP INSTRUCTIONS FOR MS ACCESS
=========================================

1. Create a new Access database file at: V:\LoginDB.accdb

2. Create a table named "Users" with the following structure:

Table Name: Users
Fields:
- ID (AutoNumber, Primary Key)
- Username (Short Text, 50 characters, Required)
- Password (Short Text, 100 characters, Required)
- FullName (Short Text, 100 characters, Required)
- UserRole (Short Text, 50 characters, Required)

3. Insert sample data:

INSERT INTO Users (Username, Password, FullName, UserRole)
VALUES 
('admin', 'admin123', 'Administrator', 'Admin'),
('user1', 'user123', 'John Doe', 'User'),
('manager', 'manager123', 'Jane Smith', 'Manager');

SQL Script (can be run in Access Query Designer):
================================================

CREATE TABLE Users (
    ID COUNTER PRIMARY KEY,
    Username TEXT(50) NOT NULL,
    Password TEXT(100) NOT NULL,
    FullName TEXT(100) NOT NULL,
    UserRole TEXT(50) NOT NULL
);

INSERT INTO Users (Username, Password, FullName, UserRole)
VALUES ('admin', 'admin123', 'Administrator', 'Admin');

INSERT INTO Users (Username, Password, FullName, UserRole)
VALUES ('user1', 'user123', 'John Doe', 'User');

INSERT INTO Users (Username, Password, FullName, UserRole)
VALUES ('manager', 'manager123', 'Jane Smith', 'Manager');

Note: In production, passwords should be hashed for security.

TESTING CREDENTIALS:
===================
Username: admin    | Password: admin123
Username: user1    | Password: user123  
Username: manager  | Password: manager123
*/