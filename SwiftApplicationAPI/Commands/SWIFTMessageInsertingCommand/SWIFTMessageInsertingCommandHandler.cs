using MediatR;
using SwiftApplicationAPI.Controllers;
using SwiftApplicationAPI.Models.ParseMTModels;
using SwiftApplicationAPI.Services;
using System.Text;
using System.Windows.Input;

namespace SwiftApplicationAPI.Queries.GetSwiftMessage
{
    public record SWIFTMessageInsertingCommand(IFormFile swiftFile) : IRequest<int>;

    public class SWIFTMessageInsertingCommandHandler : IRequestHandler<SWIFTMessageInsertingCommand, int>
    {
        private readonly ISwiftParserService<MT799Model> swiftParserService;
        private readonly ISwiftMessageRepository swiftMessageRepository;
        private readonly ILogger<SWIFTMessageInsertingCommandHandler> logger;

        public SWIFTMessageInsertingCommandHandler(ISwiftParserService<MT799Model> swiftParserService, ISwiftMessageRepository swiftMessageRepository, ILogger<SWIFTMessageInsertingCommandHandler> logger)
        {
            this.swiftParserService = swiftParserService;
            this.swiftMessageRepository = swiftMessageRepository;
            this.logger = logger;
        }
        public async Task<int> Handle(SWIFTMessageInsertingCommand command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Reading through the SWIFT File");

            var swiftContent = new StringBuilder();
            using (var reader = new StreamReader(command.swiftFile.OpenReadStream()))
            {
                swiftContent.Append(reader.ReadToEnd());
            }

            logger.LogInformation("Reading completed succesfully.Now begging Parsing");

            var parsedResult = await swiftParserService.Parser(swiftContent.ToString());

            logger.LogInformation("Inserting in the table");
            var result = await swiftMessageRepository.Create(parsedResult);
            logger.LogInformation($"Inserting completed, numbers of row affected :{result}");

            return result;
        }
    }
}
