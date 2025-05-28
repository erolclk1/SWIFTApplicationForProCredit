using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SwiftApplicationAPI.Services;
using System.Text;
using SwiftApplicationAPI.Services;
using MediatR;
using SwiftApplicationAPI.Queries.GetSwiftMessage;
using SwiftApplicationAPI.Models.ParseMTModels;
using SwiftApplicationAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace SwiftApplicationAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class MTSwiftGeneratorController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly ILogger<MTSwiftGeneratorController> logger;

        public MTSwiftGeneratorController(IMediator mediator, ILogger<MTSwiftGeneratorController> logger)
        {
            this.mediator = mediator;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<int> SWIFTMT103MessageGenerator([FromBody] MT103ModelDTO swiftInput)
        {
            try
            {
                logger.LogInformation("Sending a SwiftMessageInsertingCommand");
                var result = await mediator.Send(new MT103MessageGeneratorCommand(swiftInput));
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError("Something went wrong in calling the method SWIFTMessageInserting", ex);
                throw new Exception(ex.Message);
            }

        }

        [HttpPost]
        public async Task<int> SWIFTMT799MessageGenerator(MT799ModelDTO swiftInput)
        {
            try
            {
                logger.LogInformation("Sending a SwiftMessageInsertingCommand");
                var result = await mediator.Send(new MT799MessageGeneratorCommand(swiftInput));
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
