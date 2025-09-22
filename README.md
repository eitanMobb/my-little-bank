# My Little Bank - Personal Banking Application

## Overview

This is a C# ASP.NET Core banking web application that provides a simple and secure way to manage your personal finances. The application allows users to:

- Log in with username/password
- View their checking and savings accounts
- Transfer money between accounts
- Search for accounts

## Features

### Account Management
- Secure user authentication
- Multiple account types (Checking and Savings)
- Real-time balance tracking
- Account history and details

### Money Transfers
- Transfer funds between your own accounts
- Real-time balance updates
- Transaction validation and error handling

### Search Functionality
- Quick account lookup
- Search by account number or customer name
- Comprehensive account information display

## Sample Accounts

The application comes with pre-loaded sample accounts for testing:

| Username | Password | Accounts |
|----------|----------|----------|
| john.doe | password123 | CHK001 ($2,500), SAV001 ($15,000) |
| jane.smith | mypassword | CHK002 ($3,200.50), SAV002 ($8,500) |
| admin | admin123 | CHK003 ($10,000), SAV003 ($50,000) |

## Prerequisites

- .NET 9.0 SDK or later
- A web browser

## How to Run

1. **Clone or download** this project to your local machine

2. **Navigate to the project directory**:
   ```bash
   cd "photo album"
   ```

3. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

4. **Run the application**:
   ```bash
   dotnet run
   ```

5. **Open your browser** and navigate to:
   ```
   https://localhost:5001
   ```
   or
   ```
   http://localhost:5000
   ```

6. **Login** using one of the sample accounts listed above

## Technology Stack

- **Backend**: ASP.NET Core 9.0
- **Database**: SQLite
- **Authentication**: Cookie-based authentication
- **Frontend**: Razor Pages with Bootstrap
- **Security**: HTTPS support, secure authentication

## Database

The application uses SQLite with a local database file (`bank.db`) that is automatically created when the application starts. The database contains:

- **Users table**: User credentials and personal information
- **Accounts table**: Bank account information linked to users

## Getting Started

1. **Login**: Use one of the provided sample accounts to access the system
2. **Dashboard**: View your account balances and recent activity
3. **Transfer**: Move money between your checking and savings accounts
4. **Search**: Look up account information using the search functionality

## Development

This application is built using modern ASP.NET Core practices:

- MVC architecture
- Dependency injection
- Entity Framework for data access
- Secure authentication and authorization
- Responsive web design

## Support

For questions or support, please contact the development team.

## License

This application is for educational and demonstration purposes.