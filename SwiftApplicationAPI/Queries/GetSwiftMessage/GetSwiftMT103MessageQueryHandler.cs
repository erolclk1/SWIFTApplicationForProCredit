using MediatR;
using SwiftApplicationAPI.Controllers;
using SwiftApplicationAPI.Models.ParseMTModels;
using SwiftApplicationAPI.Services.ParserServices;
using System.Text;

namespace SwiftApplicationAPI.Queries.GetSwiftMessage
{
    public record GetSwiftMT103MessageQuery(IFormFile swiftFile) : IRequest<MT103Model>;

    public class GetSwiftMT103MessageQueryHandler : IRequestHandler<GetSwiftMT103MessageQuery, MT103Model>
    {
        private readonly ISwiftParserService<MT103Model> swiftParserService;
        private readonly ILogger<GetSwiftMT103MessageQueryHandler> logger;

        public GetSwiftMT103MessageQueryHandler(ISwiftParserService<MT103Model> swiftParserService, ILogger<GetSwiftMT103MessageQueryHandler> logger)
        {
            this.swiftParserService = swiftParserService;
            this.logger = logger;
        }
        public async Task<MT103Model> Handle(GetSwiftMT103MessageQuery query, CancellationToken cancellationToken)
        {
            logger.LogInformation("Reading through the SWIFT File");

            var swiftContent = new StringBuilder();
            using (var reader = new StreamReader(query.swiftFile.OpenReadStream()))
            {
                swiftContent.Append(reader.ReadToEnd());
            }

            logger.LogInformation("Reading completed succesfully.Now begging Parsing");

            var result = await swiftParserService.Parser(swiftContent.ToString());
            return result;
        }
    }
}
