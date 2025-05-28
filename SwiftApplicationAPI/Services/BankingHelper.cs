namespace SwiftApplicationAPI.Services
{
    public class BankingHelper
    {
        private static readonly HashSet<string> EuropeanCountryCodes = new()
    {
        "BG", "DE", "FR", "ES", "IT", "NL", "PL", "RO", "GR", "PT", "SE",
    };

        private static readonly Dictionary<string, string> CountryCurrencyMap = new()
    {
        { "BG", "BGN" }, { "DE", "EUR" }, { "US", "USD" }, { "UK", "GBP" },
    };
        private static readonly Dictionary<string, string> CountryBICMap = new()
        {
            { "BG", "BNBGBUGG" }, // Bulgarian National Bank
            { "DE", "DEUTDEFF" }, // Deutsche Bank
            { "FR", "BNPAFRPP" }, // BNP Paribas
            { "US", "BOFAUS3N" }, // Bank of America
            { "UK", "BARCGB22" }, // Barclays
            { "IN", "SBININBB" }, // State Bank of India
            { "CA", "ROYCCAT2" }, // Royal Bank of Canada
        };

        public static string GetBIC(string countryCode)
        {
            return CountryBICMap.TryGetValue(countryCode.ToUpper(), out var bic)
                ? bic
                : "BICCODE"; // fallback BIC
        }

        public static bool IsEuropean(string countryCode) =>
            EuropeanCountryCodes.Contains(countryCode.ToUpper());

        public static string GenerateIBAN(string countryCode)
        {
            var random = new Random();
            var bankCode = "BANK"; // Simulate bank code
            var accountNumber = random.Next(10000000, 99999999);
            return $"{countryCode.ToUpper()}00{bankCode}{accountNumber}";
        }

        public static string GenerateBIC(string countryCode)
        {
            var random = new Random();
            var accountNumber = random.Next(100, 999);
            var bankCode = GetBIC(countryCode);
            return $"{bankCode}{countryCode.ToUpper()}{bankCode}";
        }

        public static string GetCurrency(string countryCode) {
            CountryCurrencyMap.TryGetValue(countryCode.ToUpper(), out var currency);
            return currency; }
    }
}

