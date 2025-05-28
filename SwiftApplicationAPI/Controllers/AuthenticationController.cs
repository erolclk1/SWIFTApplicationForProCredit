using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SwiftApplicationAPI.Services;
using System.Text;
using SwiftApplicationAPI.Services;
using MediatR;
using SwiftApplicationAPI.Queries.GetSwiftMessage;
using SwiftApplicationAPI.Models.ParseMTModels;
using SwiftApplicationAPI.Models;
using Microsoft.AspNetCore.Identity.Data;
using SwiftApplicationAPI.Models.Users;
using SwiftApplicationAPI.Models.CommandDtos;

namespace SwiftApplicationAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly ILogger<AuthenticationController> logger;

        public AuthenticationController(IMediator mediator, ILogger<AuthenticationController> logger)
        {
            this.mediator = mediator;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<string> Register([FromQuery]RegisterUserDTO user)
        {
            try
            {
                logger.LogInformation("Registering the User");
                var result = await mediator.Send(new RegisterUserCommand(user));
                return result.Item2;
            }
            catch (Exception ex)
            {
                logger.LogError("Something went wrong in registering the user", ex);
                throw new Exception(ex.Message);
            }

        }

        [HttpPost]
        public async Task<string> Login(string email,string password)
        {
            try
            {
                logger.LogInformation("Sending a SwiftMessageInsertingCommand");
                var result = await mediator.Send(new LoginUserCommand(email,password));
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError("Something went wrong in calling the method SWIFTMessageInserting", ex);
                throw new Exception(ex.Message);
            }

        }
    }
}
