using Dapper;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SwiftApplicationAPI.Data;
using SwiftApplicationAPI.Models.CommandDtos;
using SwiftApplicationAPI.Models.Users;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SwiftApplicationAPI.Services.AuthenticationServices
{
    public class UserServices : IUserServices
    {
        private readonly SWIFTMessagesDataContext dataContext;
        private readonly IPasswordHasher<UserModel> passwordHasher;
        private readonly IConfiguration config;

        public UserServices(SWIFTMessagesDataContext dataContext, IPasswordHasher<UserModel> passwordHasher, IConfiguration config)
        {
            this.dataContext = dataContext;
            this.passwordHasher = passwordHasher;
            this.config = config;
        }
        public async Task<(bool Success, string? Message)> RegisterUserAsync(RegisterUserDTO user)
        {
            using var db = dataContext.CreateConnection();
            db.Open();
            using (var transaction = db.BeginTransaction())
            {
                try
                {
                    var existingUser = await db.QueryFirstOrDefaultAsync<UserModel>(
                         "SELECT * FROM Users WHERE Email = @Email",
                         new { Email = user.Email }
                     ,transaction);
                    if (existingUser != null) return (false, "User already exist");

                    var isEuropean = BankingHelper.IsEuropean(user.CountryCode);
                    var ibanOrBic = isEuropean
                        ? BankingHelper.GenerateIBAN(user.CountryCode)
                        : BankingHelper.GenerateBIC(user.CountryCode);
                    var currency = BankingHelper.GetCurrency(user.CountryCode);

                    var userModel = new UserModel
                    {
                        Name = user.Name,
                        Email = user.Email,
                        PasswordHash = passwordHasher.HashPassword(null!, user.Password),
                        CountryCode = user.CountryCode.ToUpper(),
                        CreatedAt = DateTime.UtcNow,
                    };
                    string sql = @"INSERT INTO Users (Name, Email, CountryCode, CreatedAt, PasswordHash)
               VALUES (@Name, @Email, @CountryCode, @CreatedAt, @PasswordHash);
               SELECT LAST_INSERT_ID();";
                    var dbexecute = await db.QueryAsync<int>(sql, userModel, transaction);

                    var bankModel = new BankModel
                    {
                        UserId = dbexecute.FirstOrDefault(),
                        IBANOrBIC = ibanOrBic,
                        Currency = currency,
                        Balance = 0,
                        CreatedAt = DateTime.UtcNow,
                    };
                    string sql2 = @"INSERT INTO BankAccounts (UserId, IBANOrBIC, Currency, Balance, CreatedAt)
               VALUES (@UserId, @IBANOrBIC, @Currency, @Balance, @CreatedAt);";
                    var dbexecute2 = await db.ExecuteAsync(sql2, bankModel, transaction);

                    transaction.Commit();
                    db.Close();
                    return (true, "Registering User was successful");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Something went wrong : " + ex);
                }
            }
        }

        public async Task<string> Login(string email, string password)
        {
            using var db = dataContext.CreateConnection();
            var user = await db.QueryFirstOrDefaultAsync<UserModel>(
                "SELECT * FROM Users WHERE Email = @email",
                new { email }
            );

            if (user == null) return null;

            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result != PasswordVerificationResult.Success) return null;

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(UserModel user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(config["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(JwtRegisteredClaimNames.Aud, "SwiftApplicationAPI"),
            new Claim(JwtRegisteredClaimNames.Iss, "SwiftApplicationAPI")
            }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
