using SwiftApplicationAPI.Models.ParseMTModels;

namespace SwiftApplicationAPI.Services.Currency
{
    public interface ICurrencyConverterService
    {
        public Task<decimal> extractMoneyAndConvert(MT103Model? mt103Message);
        public Task<decimal> ConvertAsync(string fromCurrency, string toCurrency, decimal amount);
    }
}
