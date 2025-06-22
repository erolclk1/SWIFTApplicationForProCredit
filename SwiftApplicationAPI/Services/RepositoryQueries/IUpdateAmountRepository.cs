using SwiftApplicationAPI.Models.ParseMTModels;
using System.Data;

namespace SwiftApplicationAPI.Services.RepositoryQueries
{
    public interface IUpdateAmountRepository
    {
        public Task<(bool Success, string Message)> updateSenderAmount(decimal ammount,string IBanOrBic, IDbConnection db, IDbTransaction transaction);
        public Task<(bool Success, string Message)> updateTransactionHistory(string TransactionID, string SenderIBanOrBic, string RecieverstringIBanOrBic, decimal originalAmmount, decimal convertedAmount, IDbConnection db, IDbTransaction transaction);
        public Task<(bool Success, string Message)> updateRecieverAmount(decimal convertedAmount ,string IBanOrBic, IDbConnection db, IDbTransaction transaction);
        public Task<(bool Success, string Message)> transferTransaction(MT103Model mt103Message, decimal convertedAmount);
    }
}
