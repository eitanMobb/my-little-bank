using Microsoft.Data.Sqlite;
using MyLittleBank.Models;

namespace MyLittleBank.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            _connectionString = "Data Source=bank.db";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Create Users table
            var createUsersTable = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL,
                    FirstName TEXT NOT NULL,
                    LastName TEXT NOT NULL,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                )";

            // Create Accounts table
            var createAccountsTable = @"
                CREATE TABLE IF NOT EXISTS Accounts (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    AccountType TEXT NOT NULL,
                    Balance DECIMAL(18,2) DEFAULT 0.00,
                    AccountNumber TEXT NOT NULL UNIQUE,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (UserId) REFERENCES Users (Id)
                )";

            using var command1 = new SqliteCommand(createUsersTable, connection);
            command1.ExecuteNonQuery();

            using var command2 = new SqliteCommand(createAccountsTable, connection);
            command2.ExecuteNonQuery();

            // Insert sample data
            InsertSampleData(connection);
        }

        private void InsertSampleData(SqliteConnection connection)
        {
            // Check if data already exists
            var checkUsers = "SELECT COUNT(*) FROM Users";
            using var checkCommand = new SqliteCommand(checkUsers, connection);
            var userCount = Convert.ToInt32(checkCommand.ExecuteScalar());

            if (userCount == 0)
            {
                // Insert sample users
                var insertUsers = @"
                    INSERT INTO Users (Username, Password, FirstName, LastName) VALUES
                    ('john.doe', 'password123', 'John', 'Doe'),
                    ('jane.smith', 'mypassword', 'Jane', 'Smith'),
                    ('admin', 'admin123', 'Admin', 'User')";

                using var userCommand = new SqliteCommand(insertUsers, connection);
                userCommand.ExecuteNonQuery();

                // Insert sample accounts
                var insertAccounts = @"
                    INSERT INTO Accounts (UserId, AccountType, Balance, AccountNumber) VALUES
                    (1, 'Checking', 2500.00, 'CHK001'),
                    (1, 'Savings', 15000.00, 'SAV001'),
                    (2, 'Checking', 3200.50, 'CHK002'),
                    (2, 'Savings', 8500.00, 'SAV002'),
                    (3, 'Checking', 10000.00, 'CHK003'),
                    (3, 'Savings', 50000.00, 'SAV003')";

                using var accountCommand = new SqliteCommand(insertAccounts, connection);
                accountCommand.ExecuteNonQuery();
            }
        }

        public User? AuthenticateUser(string username, string password)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var query = $"SELECT * FROM Users WHERE Username = '{username}' AND Password = '{password}'";
            
            using var command = new SqliteCommand(query, connection);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Password = reader.GetString(2),
                    FirstName = reader.GetString(3),
                    LastName = reader.GetString(4),
                    CreatedAt = reader.GetDateTime(5)
                };
            }

            return null;
        }

        public List<Account> GetUserAccounts(int userId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var query = $"SELECT * FROM Accounts WHERE UserId = {userId}";
            
            using var command = new SqliteCommand(query, connection);
            using var reader = command.ExecuteReader();

            var accounts = new List<Account>();
            while (reader.Read())
            {
                accounts.Add(new Account
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    AccountType = reader.GetString(2),
                    Balance = reader.GetDecimal(3),
                    AccountNumber = reader.GetString(4),
                    CreatedAt = reader.GetDateTime(5)
                });
            }

            return accounts;
        }

        // Get account information by account number
        public Account? GetAccountByNumber(string accountNumber)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Query account by account number
            var query = $"SELECT * FROM Accounts WHERE AccountNumber = '{accountNumber}'";
            
            using var command = new SqliteCommand(query, connection);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Account
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    AccountType = reader.GetString(2),
                    Balance = reader.GetDecimal(3),
                    AccountNumber = reader.GetString(4),
                    CreatedAt = reader.GetDateTime(5)
                };
            }

            return null;
        }

        // Transfer money between accounts
        public bool TransferMoney(string fromAccountNumber, string toAccountNumber, decimal amount)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                // Get from account
                var fromQuery = $"SELECT * FROM Accounts WHERE AccountNumber = '{fromAccountNumber}'";
                using var fromCommand = new SqliteCommand(fromQuery, connection, transaction);
                using var fromReader = fromCommand.ExecuteReader();

                if (!fromReader.Read())
                {
                    transaction.Rollback();
                    return false;
                }

                var fromBalance = fromReader.GetDecimal(3);
                fromReader.Close();

                if (fromBalance < amount)
                {
                    transaction.Rollback();
                    return false;
                }

                // Update from account
                var updateFromQuery = $"UPDATE Accounts SET Balance = Balance - {amount} WHERE AccountNumber = '{fromAccountNumber}'";
                using var updateFromCommand = new SqliteCommand(updateFromQuery, connection, transaction);
                updateFromCommand.ExecuteNonQuery();

                // Update to account
                var updateToQuery = $"UPDATE Accounts SET Balance = Balance + {amount} WHERE AccountNumber = '{toAccountNumber}'";
                using var updateToCommand = new SqliteCommand(updateToQuery, connection, transaction);
                updateToCommand.ExecuteNonQuery();

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }

        public List<Account> SearchAccounts(string searchTerm)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var query = $"SELECT a.*, u.FirstName, u.LastName FROM Accounts a JOIN Users u ON a.UserId = u.Id WHERE u.FirstName LIKE '%{searchTerm}%' OR u.LastName LIKE '%{searchTerm}%' OR a.AccountNumber LIKE '%{searchTerm}%'";
            
            using var command = new SqliteCommand(query, connection);
            using var reader = command.ExecuteReader();

            var accounts = new List<Account>();
            while (reader.Read())
            {
                accounts.Add(new Account
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    AccountType = reader.GetString(2),
                    Balance = reader.GetDecimal(3),
                    AccountNumber = reader.GetString(4),
                    CreatedAt = reader.GetDateTime(5)
                });
            }

            return accounts;
        }
    }
}
