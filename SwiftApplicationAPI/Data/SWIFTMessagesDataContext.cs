using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Data;

namespace SwiftApplicationAPI.Data
{
    public class SWIFTMessagesDataContext
    {
        private readonly IConfiguration configuration;

        public SWIFTMessagesDataContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public IDbConnection CreateConnection()
        {
            //return new SqliteConnection(configuration.GetConnectionString("SwiftDatabaseSqlLiteString")); old one with SQLite
            return new MySqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task Init()
        { 
        
            using var connection =  CreateConnection();
            var sql = """
                CREATE TABLE IF NOT EXISTS 
                SWIFTMessage (
                    Id INTEGER NOT NULL PRIMARY KEY AUTO_INCREMENT,
                    BasicHeader TEXT,
                    ApplicationHeader TEXT,
                    UserHeader TEXT,
                    Text TEXT,
                    Tailers TEXT
                );
            CREATE TABLE IF NOT EXISTS Users (
                UserId INTEGER PRIMARY KEY AUTO_INCREMENT,
                Name TEXT NOT NULL,
                Email TEXT NOT NULL,
                CountryCode TEXT NOT NULL,
                PasswordHash VARCHAR(256) NOT NULL,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
            );
            CREATE TABLE IF NOT EXISTS BankAccounts (
                AccountId INTEGER PRIMARY KEY AUTO_INCREMENT,
                UserId INTEGER NOT NULL UNIQUE,
                IBANOrBIC TEXT NOT NULL,
                Balance DECIMAL(18, 4) NOT NULL DEFAULT 0,
                Currency TEXT NOT NULL,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
            );
            CREATE TABLE IF NOT EXISTS Transactions (
                TransactionId VARCHAR(256) NOT NULL PRIMARY KEY,
                SenderId INTEGER NOT NULL,
                ReceiverId INTEGER NOT NULL,
                RecieverAmount DECIMAL(18, 4) NOT NULL,
                RecieverCurrency CHAR(3) NOT NULL,
                OriginalAmount DECIMAL(18, 4) NOT NULL,
                OriginalCurrency CHAR(3) NOT NULL,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY(SenderId) REFERENCES Users(UserId),
                FOREIGN KEY(ReceiverId) REFERENCES Users(UserId)
            );
            """;
            await connection.ExecuteAsync(sql);
        }

    }
}
