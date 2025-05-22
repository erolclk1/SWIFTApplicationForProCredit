using System.Text.RegularExpressions;
using SwiftApplicationAPI.Models.ParseMTModels;

namespace SwiftApplicationAPI.Services
{
    public class MT103ParserService : ISwiftParserService<MT103Model>
    {
        private readonly ILogger<MT103ParserService> logger;

        public MT103ParserService(ILogger<MT103ParserService> logger)
        {
            this.logger = logger;
        }

        public async Task<MT103Model> Parser(string swiffContent)
        {
            try
            {
                logger.LogInformation("Parsing the SWIFT FILE");

                var blockMatches = Regex.Matches(swiffContent, @"\{[0-9]+:[^{}]*?(?:\{[^{}]*\})*\}", RegexOptions.Singleline);
                var blocks = blockMatches.Select(x => x.Value).ToList();

                var block4 = GetBlockContent(blocks, "4").TrimEnd('-');
                var tagMatches = Regex.Matches(block4, @":([0-9]{2}[A-Z]?):(.*?)(?=:[0-9]{2}[A-Z]?:|$)", RegexOptions.Singleline);


                var mtData = new MT103Model
                {
                    BasicHeader = GetBlockContent(blocks, "1"),
                    ApplicationHeader = GetBlockContent(blocks, "2"),
                    UserHeader = GetBlockContent(blocks, "3"),
                    Text = GetBlockContent(blocks, "4"),
                    Tailers = GetBlockContent(blocks, "5")
                };

                ParseBlock4Content(mtData.Text, mtData);

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
        private void ParseBlock4Content(string block4, MT103Model mtData)
        {
            var tagMatches = Regex.Matches(block4, @":(\d{2}[A-Z]?):([\s\S]*?)(?=:\d{2}[A-Z]?:|$)");

            var tagMap = new Dictionary<string, Action<string>>
    {
        { "20",  val => mtData.TransactionReference = val },
        { "23B", val => mtData.BankOperationCode = val },
        { "32A", val => mtData.ValueDateCurrencyAmount = val },
        { "50K", val => mtData.OrderingCustomer = val },
        { "59",  val => mtData.BeneficiaryCustomer = val },
        { "70",  val => mtData.RemittanceInformation = val },
        { "71A", val => mtData.DetailsOfCharges = val }
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
