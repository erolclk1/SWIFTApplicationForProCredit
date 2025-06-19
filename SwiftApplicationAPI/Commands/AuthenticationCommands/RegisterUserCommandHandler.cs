using MediatR;
using SwiftApplicationAPI.Controllers;
using SwiftApplicationAPI.Models;
using SwiftApplicationAPI.Models.CommandDtos;
using SwiftApplicationAPI.Models.ParseMTModels;
using SwiftApplicationAPI.Models.Users;
using SwiftApplicationAPI.Services.AuthenticationServices;
using SwiftApplicationAPI.Services.KafkaServices;
using System.Text;
using System.Windows.Input;

namespace SwiftApplicationAPI.Queries.GetSwiftMessage
{
    public record RegisterUserCommand(RegisterUserDTO user) : IRequest<(bool, string)>;

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, (bool,string)>
    {
        private readonly ILogger<RegisterUserCommandHandler> logger;
        private readonly IUserServices userServices;

        public RegisterUserCommandHandler(ILogger<RegisterUserCommandHandler> logger,IUserServices userServices)
        {
            this.logger = logger;
            this.userServices = userServices;
        }
        public async Task<(bool,string)> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            var (success, message) = await userServices.RegisterUserAsync(command.user);
            logger.LogInformation("Register Successful");
            return (success,message);
        }
    }
}
