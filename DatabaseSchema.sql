CREATE DATABASE BankManagementSystem;
GO

USE BankManagementSystem;
GO

CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100),
    Username NVARCHAR(50) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20),
    Phone NVARCHAR(20),
    Email NVARCHAR(100),
    CreatedAt DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);
GO

CREATE TABLE Customers (
    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    CustomerCode NVARCHAR(20) UNIQUE,
    FullName NVARCHAR(100),
    Gender NVARCHAR(10),
    DateOfBirth DATE,
    Phone NVARCHAR(20),
    Email NVARCHAR(100),
    Address NVARCHAR(200),
    NationalID NVARCHAR(50),
    Status NVARCHAR(20) DEFAULT 'Active',
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE Accounts (
    AccountID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    AccountNumber NVARCHAR(30) UNIQUE NOT NULL,
    AccountType NVARCHAR(20),
    Balance DECIMAL(18,2) DEFAULT 0.00,
    Status NVARCHAR(20) DEFAULT 'Active',
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE Transactions (
    TransactionID INT IDENTITY(1,1) PRIMARY KEY,
    AccountID INT FOREIGN KEY REFERENCES Accounts(AccountID),
    TransactionType NVARCHAR(20),
    Amount DECIMAL(18,2),
    Description NVARCHAR(200),
    TransactionDate DATETIME DEFAULT GETDATE()
);
GO

-- Insert default Admin user (Password: admin123 -> you should hash this in real scenarios but keeping simple here if needed)
-- INSERT INTO Users (FullName, Username, PasswordHash, Role, Phone, Email, IsActive) 
-- VALUES ('System Admin', 'admin', 'admin123', 'Admin', '1234567890', 'admin@bank.com', 1);
-- GO
