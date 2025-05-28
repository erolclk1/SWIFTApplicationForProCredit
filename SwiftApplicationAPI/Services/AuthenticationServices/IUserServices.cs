using SwiftApplicationAPI.Models.CommandDtos;
using SwiftApplicationAPI.Models.Users;

namespace SwiftApplicationAPI.Services.AuthenticationServices
{
    public interface IUserServices
    {
        public Task<(bool Success, string? Message)> RegisterUserAsync(RegisterUserDTO user);
        public Task<string> Login(string email, string password);
    }
}
