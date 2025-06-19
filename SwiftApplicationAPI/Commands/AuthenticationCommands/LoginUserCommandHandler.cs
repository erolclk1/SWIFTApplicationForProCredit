using MediatR;
using SwiftApplicationAPI.Controllers;
using SwiftApplicationAPI.Models;
using SwiftApplicationAPI.Models.ParseMTModels;
using SwiftApplicationAPI.Models.Users;
using SwiftApplicationAPI.Services.AuthenticationServices;
using SwiftApplicationAPI.Services.KafkaServices;
using System.Text;
using System.Windows.Input;

namespace SwiftApplicationAPI.Queries.GetSwiftMessage
{
    public record LoginUserCommand(string email,string password) : IRequest<string>;

    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, string> {
        private readonly ILogger<LoginUserCommandHandler> logger;
        private readonly IUserServices userServices;

        public LoginUserCommandHandler(ILogger<LoginUserCommandHandler> logger,IUserServices userServices)
        {
            this.logger = logger;
            this.userServices = userServices;
        }
        public async Task<string> Handle(LoginUserCommand command, CancellationToken cancellationToken)
        {
            var  jwtToken = await userServices.Login(command.email, command.password);
            logger.LogInformation("Register Successful");
            return jwtToken;
        }
    }
}
