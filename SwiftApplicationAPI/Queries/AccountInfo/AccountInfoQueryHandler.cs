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
    public record AccountInfoQuery() : IRequest<AccountInfoDTO>;

    public class AccountInfoQueryHandler : IRequestHandler<AccountInfoQuery, AccountInfoDTO>
    {
        private readonly ILogger<AccountInfoQueryHandler> logger;
        private readonly SWIFTMessagesDataContext dataContext;
        private readonly IHttpContextTokenAccessorService httpContextTokenAccessor;
        public AccountInfoQueryHandler(ILogger<AccountInfoQueryHandler> logger, SWIFTMessagesDataContext dataContext, IHttpContextTokenAccessorService httpContextTokenAccessor)
        {
            this.logger = logger;
            this.dataContext = dataContext;
            this.httpContextTokenAccessor = httpContextTokenAccessor;
        }

        public async Task<AccountInfoDTO> Handle(AccountInfoQuery request, CancellationToken cancellationToken)
        {
            await httpContextTokenAccessor.SetToken();
            var db = dataContext.CreateConnection();
            var jwtToken = await httpContextTokenAccessor.GetToken();
            var userId = await GetUserIdFromToken(jwtToken);
            var accountInfoDTOs = await db.QueryAsync<AccountInfoDTO>(sqlQuery, new { UserId = userId });
            return accountInfoDTOs.FirstOrDefault();
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

        private string sqlQuery = @"
                SELECT 
                    u.Name AS UserName,
                    u.Email,
                    u.CountryCode,
                    b.Balance,
                    b.IBANOrBIC,
                    b.Currency
                FROM Users u
                JOIN BankAccounts b ON u.UserId = b.UserId
                WHERE u.UserId = @UserId;
            ";
    }
}
