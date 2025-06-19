
using Dapper;
using SwiftApplicationAPI.Data;
using System.Data;
using SwiftApplicationAPI.Models.ParseMTModels;

namespace SwiftApplicationAPI.Services
{
    public class SwiftMessageRepository : ISwiftMessageRepository
    {
        private readonly SWIFTMessagesDataContext dataContext;
        private readonly ILogger<SwiftMessageRepository> logger;

        public SwiftMessageRepository(SWIFTMessagesDataContext dataContext , ILogger<SwiftMessageRepository> logger)
        {
            this.dataContext = dataContext;
            this.logger = logger;
        }
        public async Task<int> Create(MT799Model mT799Model)
        {
            logger.LogInformation("Creating connection with the dbContext");
            using var connection = dataContext.CreateConnection();
            var sql = """
            INSERT INTO SWIFTMessage (BasicHeader, ApplicationHeader, UserHeader, Text, Tailers)
            VALUES (@BasicHeader, @ApplicationHeader, @UserHeader, @Text, @Tailers)
        """;
            return await connection.ExecuteAsync(sql, mT799Model);
        }
    }
}
