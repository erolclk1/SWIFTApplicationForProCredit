namespace SwiftApplicationAPI.Models.ParseMTModels
{
    public class MT103Model
    {
        public string BasicHeader { get; set; } //block 1
        public string ApplicationHeader { get; set; } //block 2
        public string UserHeader { get; set; } // block 3
        public string Text { get; set; }
        public string Tailers { get; set; } //Block 5 if it exist

        //block 4 TEXT PARSED
        public string TransactionReference { get; set; }
        public string BankOperationCode { get; set; }
        public string ValueDateCurrencyAmount { get; set; }
        public string OrderingCustomer { get; set; }
        public string BeneficiaryCustomer { get; set; }
        public string RemittanceInformation { get; set; }
        public string DetailsOfCharges { get; set; }
        //end of block 4 TEXT
    }
}
