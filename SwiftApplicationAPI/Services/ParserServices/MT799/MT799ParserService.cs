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

                ParseBlock4Content(mtData.Text,mtData);

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
        private void ParseBlock4Content(string block4, MT799Model mtData)
        {
            var tagMatches = Regex.Matches(block4, @":(\d{2}[A-Z]?):([\s\S]*?)(?=:\d{2}[A-Z]?:|$)");

            var tagMap = new Dictionary<string, Action<string>>
    {
        { "20",  val => mtData.TransactionReference = val },
        { "79", val => mtData.NarrativeMessage = val },
    };

            tagMatches
                .Cast<Match>()
                .Select(m => new
                {
                    Tag = m.Groups[1].Value.Trim(),
                    Value = m.Groups[2].Value.Trim()
                })
                .ToList()
                .ForEach(tag =>
                {
                    if (tagMap.TryGetValue(tag.Tag, out var setter))
                    {
                        setter(tag.Value);
                    }
                });
        }
    }
}
