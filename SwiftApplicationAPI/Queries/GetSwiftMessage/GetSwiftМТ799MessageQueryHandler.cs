using MediatR;
using SwiftApplicationAPI.Controllers;
using SwiftApplicationAPI.Models.ParseMTModels;
using SwiftApplicationAPI.Services.ParserServices;
using System.Text;

namespace SwiftApplicationAPI.Queries.GetSwiftMessage
{
    public record GetSwiftМТ799MessageQuery(IFormFile swiftFile) : IRequest<MT799Model>;

    public class GetSwiftМТ799MessageQueryHandler : IRequestHandler<GetSwiftМТ799MessageQuery, MT799Model>
    {
        private readonly ISwiftParserService<MT799Model> swiftParserService;
        private readonly ILogger<GetSwiftМТ799MessageQueryHandler> logger;

        public GetSwiftМТ799MessageQueryHandler(ISwiftParserService<MT799Model> swiftParserService, ILogger<GetSwiftМТ799MessageQueryHandler> logger)
        {
            this.swiftParserService = swiftParserService;
            this.logger = logger;
        }
        public async Task<MT799Model> Handle(GetSwiftМТ799MessageQuery query, CancellationToken cancellationToken)
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
