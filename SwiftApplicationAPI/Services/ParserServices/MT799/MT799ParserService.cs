using System.Text.RegularExpressions;
using SwiftApplicationAPI.Models.ParseMTModels;

namespace SwiftApplicationAPI.Services.ParserServices.MT799
{
    public class MT799ParserService : ISwiftParserService<MT799Model>
    {
        private readonly ILogger<MT799ParserService> logger;

        public MT799ParserService(ILogger<MT799ParserService> logger)
        {
            this.logger = logger;
        }

        public async Task<MT799Model> Parser(string swiffContent)
        {
            try
            {
                logger.LogInformation("Parsing the SWIFT FILE");

                var blockMatches = Regex.Matches(swiffContent, @"\{[0-9]+:[^{}]*?(?:\{[^{}]*\})*\}", RegexOptions.Singleline);
                var blocks = blockMatches.Select(x => x.Value).ToList();

                var mtData = new MT799Model
                {
                    BasicHeader = GetBlockContent(blocks, "1"),
                    ApplicationHeader = GetBlockContent(blocks, "2"),
                    UserHeader = GetBlockContent(blocks, "3"),
                    Text = GetBlockContent(blocks, "4"),
                    Tailers = GetBlockContent(blocks, "5")
                };

                logger.LogInformation("Parsing Completed");

                return mtData;
            }
            catch (Exception ex)
            {
                logger.LogError("An error occured while parsing the SWIFT FILE", ex);
                throw new Exception(ex.Message);
            }
        }

        private string GetBlockContent(List<string> blocks, string identifier)
        {
            var block = blocks
                .Where(x => x.StartsWith('{' + identifier + ":"))
                .FirstOrDefault();

            if (block != null)
            {
                var content = block.Substring(identifier.Length + 2).Trim();
                if (content.EndsWith("}"))
                {
                    content = content.Substring(0, content.Length - 1);
                }
                return content.Trim();
            }

            return string.Empty;
        }
    }
}
