using Confluent.Kafka;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.RegularExpressions;

namespace SwiftApplicationAPI.Services.ParserServices
{
    public class SwiftParserHelperService : ISwiftParserHelperService
    {
        private static readonly Regex IbanRegex = new Regex(@"\b([A-Z]{2}\d{2}[A-Z0-9]{11,30})\b", RegexOptions.IgnoreCase);
        private static readonly Regex bicRegex = new Regex(@"^[A-Z0-9]{8}([A-Z0-9]{3})?$");
        private readonly Dictionary<string, string> CountryCurrencyMap = new()
                {
                    {"BG", "BGN"},  // Bulgaria
                    {"DE", "EUR"},  // Germany
                    {"FR", "EUR"},  // France
                    {"GB", "GBP"},  // United Kingdom
                    {"IN", "INR"},  // India
                    {"ES", "EUR"},  // Spain
                    {"CA","CAD"}    // Canada
                };
        private readonly HashSet<string> EuropeanCountries = new()
        {
            "BG", "DE", "FR", "GB", "ES", "IT", "NL", "BE", "LU", "IE", "AT", "PL", "CZ", "SK",
            "HU", "RO", "GR", "PT", "SE", "FI", "DK", "EE", "LV", "LT", "SI", "HR", "CY", "MT"
        };

        public async Task<string> GetIbanOrBicFromField(string modelField)
        {
            var cleanedModelField = modelField.Split('\n', StringSplitOptions.RemoveEmptyEntries)[0].TrimStart('/').Replace(" ", "").ToUpper();
            if (string.IsNullOrEmpty(cleanedModelField))
                return null;

            var match = IbanRegex.Match(cleanedModelField);
            if (match.Success)
                return match.Value;

            match = bicRegex.Match(cleanedModelField);
            if(match.Success && cleanedModelField.Length >= 6) 
                return match.Value;
            return null;
        }

        public async Task <string> GetCurrencyFromIbanOrBic(string ibanOrBic)
        {
            if (string.IsNullOrEmpty(ibanOrBic))
                return null;

            var cleaned = ibanOrBic.Split('\n', StringSplitOptions.RemoveEmptyEntries)[0].TrimStart('/').Replace(" ", "").ToUpper();

            if (cleaned.Length >= 2)
            {
                var countryCode = cleaned.Substring(0, 2);
                if (CountryCurrencyMap.TryGetValue(countryCode, out var currency))
                    return currency;
            }

            if (cleaned.Length >= 6)
            {
                var bicCountryCode = cleaned.Substring(4, 2);
                if (CountryCurrencyMap.TryGetValue(bicCountryCode, out var currency))
                    return currency;
            }

            return null;
        }

        public async Task<string> GetCountryFromIbanOrBic(string ibanOrBic)
        {
            if (string.IsNullOrEmpty(ibanOrBic) || ibanOrBic.Length < 2)
                return null;

            if (IbanRegex.IsMatch(ibanOrBic))
                return ibanOrBic.Substring(0, 2).ToUpper();

            if (bicRegex.IsMatch(ibanOrBic) && ibanOrBic.Length >= 6)
                return ibanOrBic.Substring(4, 2).ToUpper();

            return null;
        }
        public async Task<bool> IsEuropeanCountry(string countryCode)
        {
            if (string.IsNullOrEmpty(countryCode))
                return false;

            return EuropeanCountries.Contains(countryCode.ToUpper());
        }
    }
}
