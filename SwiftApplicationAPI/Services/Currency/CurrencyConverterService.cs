
using SwiftApplicationAPI.Models.ParseMTModels;
using SwiftApplicationAPI.Services.ParserServices;

namespace SwiftApplicationAPI.Services.Currency
{
    public class CurrencyConverterService : ICurrencyConverterService
    {
        private readonly HttpClient httpClient;
        private readonly string apiKey;
        private readonly string baseUrl;
        private readonly ISwiftParserHelperService swiftParserHelperService;

        public CurrencyConverterService(IHttpClientFactory factory, IConfiguration config, ISwiftParserHelperService swiftParserHelperService)
        {
            httpClient = factory.CreateClient();
            apiKey = config["CurrencyApi:ApiKey"];
            baseUrl = config["CurrencyApi:BaseUrl"];
            this.swiftParserHelperService = swiftParserHelperService;
        }

        public async Task<decimal> extractMoneyAndConvert(MT103Model? mt103Message)
        {
            var senderIBAN = await swiftParserHelperService.GetIbanOrBicFromField(mt103Message.OrderingCustomer);
            var recieverIBAN = await swiftParserHelperService.GetIbanOrBicFromField(mt103Message.BeneficiaryCustomer);
            var fromCurrency = await swiftParserHelperService.GetCurrencyFromIbanOrBic(senderIBAN);
            var toCurrency = await swiftParserHelperService.GetCurrencyFromIbanOrBic(recieverIBAN);
            var recieverCountry = await swiftParserHelperService.GetCountryFromIbanOrBic(recieverIBAN);
            var amount = mt103Message.ValueDateCurrencyAmount.Substring(9).Replace('.', ',');
            if (await swiftParserHelperService.IsEuropeanCountry(recieverCountry))
            {
                var firstTransitionToEuro = await ConvertAsync(fromCurrency, "EUR", decimal.Parse(amount));
                var result = await ConvertAsync("EUR", toCurrency, firstTransitionToEuro);
                return result;
            }
            else 
            {
                var firstTransitionToEuro = await ConvertAsync(fromCurrency, "USD", decimal.Parse(amount));
                var result = await ConvertAsync("USD", toCurrency, firstTransitionToEuro);
                return result;
            }
        }

        public async Task<decimal> ConvertAsync(string fromCurrency, string toCurrency, decimal amount)
        {
            try {
                var url = $"{baseUrl}/latest?apikey={apiKey}&currencies={toCurrency}&base_currency={fromCurrency}";
                var response = await httpClient.GetFromJsonAsync<ApiResponse>(url);

                if (response == null || response.Data == null || !response.Data.ContainsKey(toCurrency))
                    throw new Exception("Failed to get exchange rate");

                var rate = response.Data[toCurrency];
                return amount * rate;
            }
            catch(Exception ex)
            {
                throw new Exception("Failed to get exchange rate");
            }
        }
        private class ApiResponse
        {
            public Dictionary<string, decimal> Data { get; set; }
        }
    }
}
