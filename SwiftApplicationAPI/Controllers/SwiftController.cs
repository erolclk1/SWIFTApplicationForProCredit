using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SwiftApplicationAPI.Services;
using System.Text;
using SwiftApplicationAPI.Services;
using MediatR;
using SwiftApplicationAPI.Queries.GetSwiftMessage;
using SwiftApplicationAPI.Models.ParseMTModels;
using Microsoft.AspNetCore.Authorization;

namespace SwiftApplicationAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class SwiftController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly ILogger<SwiftController> logger;

        public SwiftController(IMediator mediator,ILogger<SwiftController> logger)
        {
            this.mediator = mediator;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<MT799Model> GetSwift799Message(IFormFile swiftInput)
        {
            try
            {
                logger.LogInformation("Sending the GetSwiftMessageQuery");
                var result = await mediator.Send(new GetSwiftÃ“799MessageQuery(swiftInput));
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError("Something went wrong in calling the method GetSwiftMessage", ex);
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        public async Task<MT103Model> GetSwift103Message(IFormFile swiftInput)
        {
            try
            {
                logger.LogInformation("Sending the GetSwiftMessageQuery");
                var result = await mediator.Send(new GetSwiftMT103MessageQuery(swiftInput));
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError("Something went wrong in calling the method GetSwiftMessage", ex);
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public async Task<int> SWIFTMessageInserting(IFormFile swiftInput)
        {
            try
            {
                logger.LogInformation("Sending a SwiftMessageInsertingCommand");
                var result = await mediator.Send(new SWIFTMessageInsertingCommand(swiftInput));
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
