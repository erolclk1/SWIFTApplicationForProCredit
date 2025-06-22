using Dapper;
using MediatR;
using SwiftApplicationAPI.Controllers;
using SwiftApplicationAPI.Data;
using SwiftApplicationAPI.Models.CommandDtos;
using SwiftApplicationAPI.Models.ParseMTModels;
using SwiftApplicationAPI.Models.Users;
using SwiftApplicationAPI.Services.AuthenticationServices;
using SwiftApplicationAPI.Services.ParserServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Transactions;

namespace SwiftApplicationAPI.Queries.GetSwiftMessage
{
    public record GetTransactionHistoryQuery() : IRequest<IEnumerable<LastTransactionDTO>>;

    public class GetTransactionHistoryQueryHandler : IRequestHandler<GetTransactionHistoryQuery, IEnumerable<LastTransactionDTO>>
    {
        private readonly ILogger<GetTransactionHistoryQueryHandler> logger;
        private readonly SWIFTMessagesDataContext dataContext;
        private readonly IHttpContextTokenAccessorService httpContextTokenAccessor;
        public GetTransactionHistoryQueryHandler(ILogger<GetTransactionHistoryQueryHandler> logger, SWIFTMessagesDataContext dataContext, IHttpContextTokenAccessorService httpContextTokenAccessor)
        {
            this.logger = logger;
            this.dataContext = dataContext;
            this.httpContextTokenAccessor = httpContextTokenAccessor;
        }

        public async Task<IEnumerable<LastTransactionDTO>> Handle(GetTransactionHistoryQuery request, CancellationToken cancellationToken)
        {
            await httpContextTokenAccessor.SetToken();
            var db = dataContext.CreateConnection();
            var jwtToken = await httpContextTokenAccessor.GetToken();
            var userId = await GetUserIdFromToken(jwtToken);
            var lastTransactionDTO = await db.QueryAsync<LastTransactionDTO>(sqlQuery, new { UserId = userId });
            return lastTransactionDTO;
        }
        private async Task<string> GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            foreach (var claim in jwt.Claims)
            {
                Console.WriteLine($"{claim.Type}: {claim.Value}");
            }
            return jwt.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
        }

        private string sqlQuery = $"SELECT\r\n    t.TransactionId,\r\n  CASE\r\n        WHEN t.SenderId = @userId THEN sender.Name\r\n        WHEN t.ReceiverId = @userId THEN receiver.Name\r\n    END AS YourName,    CASE\r\n        WHEN t.SenderId = @userId THEN receiver.Name\r\n        WHEN t.ReceiverId = @userId THEN sender.Name\r\n    END AS ReceiverName,\r\n    CASE\r\n        WHEN t.ReceiverId = @UserId THEN t.RecieverAmount\r\n        WHEN t.SenderId = @UserId THEN -t.OriginalAmount\r\n        ELSE 0\r\n    END AS Amount,\r\n    CASE\r\n        WHEN t.ReceiverId = @UserId THEN t.RecieverCurrency\r\n        WHEN t.SenderId = @UserId THEN t.OriginalCurrency\r\n    END AS Currency,\r\n    t.CreatedAt\r\nFROM Transactions t\r\nJOIN Users sender ON t.SenderId = sender.UserId\r\nJOIN Users receiver ON t.ReceiverId = receiver.UserId\r\nWHERE t.SenderId = @UserId OR t.ReceiverId = @UserId\r\nORDER BY t.CreatedAt DESC;";
    }
}
