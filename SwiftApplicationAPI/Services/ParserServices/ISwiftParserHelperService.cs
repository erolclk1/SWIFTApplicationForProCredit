namespace SwiftApplicationAPI.Services.ParserServices
{
    public interface ISwiftParserHelperService
    {
        public Task<string> GetIbanOrBicFromField(string modelField);
        public Task<string> GetCountryFromIbanOrBic(string ibanOrBic);
        public Task<string> GetCurrencyFromIbanOrBic(string ibanOrBic);
        public Task<bool> IsEuropeanCountry(string countryCode);
    }
}
