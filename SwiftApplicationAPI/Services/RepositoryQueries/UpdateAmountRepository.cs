using Dapper;
using SwiftApplicationAPI.Data;
using SwiftApplicationAPI.Models.ParseMTModels;
using SwiftApplicationAPI.Models.Users;
using SwiftApplicationAPI.Services.ParserServices;
using System.Data;

namespace SwiftApplicationAPI.Services.RepositoryQueries
{
    public class UpdateAmountRepository : IUpdateAmountRepository
    {
        private readonly SWIFTMessagesDataContext dataContext;
        private readonly ISwiftParserHelperService swiftParserHelperService;

        public UpdateAmountRepository(SWIFTMessagesDataContext dataContext, ISwiftParserHelperService swiftParserHelperService)
        {
            this.dataContext = dataContext;
            this.swiftParserHelperService = swiftParserHelperService;
        }
        public async Task<(bool Success, string Message)> transferTransaction(MT103Model mt103Message, decimal convertedAmount)
        {
            var senderIbanOrBic = await swiftParserHelperService.GetIbanOrBicFromField(mt103Message.OrderingCustomer);
            var recieverIbanOrBic = await swiftParserHelperService.GetIbanOrBicFromField(mt103Message.BeneficiaryCustomer);
            var sendAmount = mt103Message.ValueDateCurrencyAmount.Substring(9).Replace('.', ',');
            using (var db = dataContext.CreateConnection())
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        var isSenderUpdateSuccessful = await updateSenderAmount(decimal.Parse(sendAmount), senderIbanOrBic, db, transaction);
                        if (!isSenderUpdateSuccessful.Success)
                        {
                            transaction.Rollback();
                            db.Close();
                            return (false, $"Transaction failed with message : {isSenderUpdateSuccessful.Message}");
                        }
                        var isRecieverUpdateSucessful = await updateRecieverAmount(convertedAmount, recieverIbanOrBic, db, transaction);
                        if (!isRecieverUpdateSucessful.Success)
                        {
                            transaction.Rollback();
                            db.Close();
                            return (false, $"Transaction failed with message : {isRecieverUpdateSucessful.Message}");
                        }
                        transaction.Commit();
                        db.Close();
                        return (true, "Transaction was successful");
                    }
                    catch (Exception ex)
                    {
                        return (false, ex.Message);
                    }
                }
            }
        }
        public async Task<(bool Success, string Message)> updateRecieverAmount(decimal convertedAmount, string IBanOrBic, IDbConnection db,IDbTransaction transaction)
        {
            try
            {
                var reciever = await db.QueryFirstOrDefaultAsync<BankModel>(
                         "SELECT * FROM BankAccounts WHERE IBANOrBIC = @IBANOrBIC",
                         new { IBANOrBIC = IBanOrBic }
                     , transaction);
                if (reciever == null)
                {
                    return (false, "Unable to find the reciever user");
                }
                reciever.Balance += convertedAmount;
                var sql = "UPDATE BankAccounts SET Balance = @Balance WHERE  IBANOrBIC= @IBANOrBIC";
                var rowsAffected = await db.ExecuteAsync(sql, new { Balance = reciever.Balance, IBANOrBIC = IBanOrBic },transaction);
                if (rowsAffected > 0)
                {
                    return (true, $"Successfuly transfer the amount of converted money to the reciever.Reciever has {reciever.Balance}{reciever.Currency} , recieved {convertedAmount}{reciever.Currency}");
                }
                else
                {
                    return (false, "Something went wrong in the updating of the amount");
                }
            }
            catch (Exception ex)
            {
                {
                    return (false, ex.Message);
                }
            }
        }

        public async Task<(bool Success, string Message)> updateSenderAmount(decimal ammount, string IBanOrBic, IDbConnection db,IDbTransaction transaction)
        {
            try
            {
                var sender = await db.QueryFirstOrDefaultAsync<BankModel>(
                         "SELECT * FROM BankAccounts WHERE IBANOrBIC = @IBANOrBIC",
                         new { IBANOrBIC = IBanOrBic }
                     , transaction);
                if (sender == null)
                {
                    return (false, "Unable to find the sender user");
                }
                if (sender.Balance < ammount)
                {
                    return (false, "Insufficient funds to complete the transaction.");
                }
                sender.Balance -= ammount;
                var sql = "UPDATE BankAccounts SET Balance = @Balance WHERE  IBANOrBIC= @IBANOrBIC";
                var rowsAffected = await db.ExecuteAsync(sql, new { Balance = sender.Balance, IBANOrBIC = IBanOrBic },transaction);
                if (rowsAffected > 0)
                {
                    return (true, $"Successfuly taken the amount of money of the sender.Left {sender.Balance}{sender.Currency} , sended {ammount}{sender.Currency}");
                }
                else
                {
                    return (false, "Something went wrong in the updating of the amount");
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
