using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
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
            return new SqliteConnection(configuration.GetConnectionString("SwiftDatabaseSqlLiteString"));
        }

        public async Task Init()
        { 
        
            using var connection =  CreateConnection();
            var sql = """
                CREATE TABLE IF NOT EXISTS 
                SWIFTMessage (
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    BasicHeader TEXT,
                    ApplicationHeader TEXT,
                    UserHeader TEXT,
                    Text TEXT,
                    Tailers TEXT
                );
            CREATE TABLE IF NOT EXISTS Users (
                UserId INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Email TEXT NOT NULL,
                IBANOrBIC TEXT NOT NULL,
                CountryCode TEXT NOT NULL,
                Balance DECIMAL(18, 4) NOT NULL DEFAULT 0,
                PasswordHash VARCHAR(256) NOT NULL,
                Currency TEXT NOT NULL,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
            );
            CREATE TABLE IF NOT EXISTS Transactions (
                TransactionId INTEGER PRIMARY KEY AUTOINCREMENT,
                SenderId INTEGER NOT NULL,
                ReceiverId INTEGER NOT NULL,
                Amount DECIMAL(18, 4) NOT NULL,
                Currency CHAR(3) NOT NULL,
                OriginalAmount DECIMAL(18, 4) NOT NULL,
                OriginalCurrency CHAR(3) NOT NULL,
                ValueDate DATE NOT NULL,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY(SenderId) REFERENCES Users(UserId),
                FOREIGN KEY(ReceiverId) REFERENCES Users(UserId)
            );
            """;
            await connection.ExecuteAsync(sql);
        }

    }
}
