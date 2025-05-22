using System.Text;

namespace SwiftApplicationAPI.Models
{
    public class MT103ModelDTO
    {
        private Random random = new Random();
        private string _basicHeader;
        private string _applicationHeader;
        private string _userHeader;
        public string BasicHeader
        {
            get => _basicHeader;
            set => _basicHeader = $"{{1:F01{value}{random.Next(0, 9999).ToString("D4")}{random.Next(0, 999999).ToString("D6")}}}";
        } //block 1 {1:F01<SenderBIC>XXXX0000000000}
        public string ApplicationHeader
        {
            get => _applicationHeader;
            set => _applicationHeader = $"{{2:I0103{value}{random.Next(0, 9999).ToString("D4")}U}}";
        } //block 2 {2:I103<ReceiverBIC>XXXXN}
        public string? UserHeader { get => _userHeader; set => _userHeader = "{3:{113:TESTMT103}}"; }   // block 3
        public string TransactionReference { get; set; }    
        public string BankOperationCode { get; set; }       
        public string ValueDateCurrencyAmount { get; set; }  
        public string OrderingCustomer { get; set; }       
        public string BeneficiaryCustomer { get; set; }    
        public string? RemittanceInformation { get; set; }
        public string DetailsOfCharges { get; set; }
        // Auto-generated Text block (Block 4)
        public string Text
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine("{4:");
                sb.AppendLine($":20:{TransactionReference}");
                sb.AppendLine($":23B:{BankOperationCode}");
                sb.AppendLine($":32A:{ValueDateCurrencyAmount}");
                sb.AppendLine($":50K:{OrderingCustomer}");
                sb.AppendLine($":59:{BeneficiaryCustomer}");
                if (!string.IsNullOrWhiteSpace(RemittanceInformation))
                    sb.AppendLine($":70:{RemittanceInformation}");
                sb.AppendLine($":71A:{DetailsOfCharges}");
                sb.AppendLine("-}");
                return sb.ToString();
            }
        }
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{BasicHeader}");
            sb.AppendLine($"{ApplicationHeader}");
            sb.AppendLine($"{UserHeader}");
            sb.AppendLine($"{Text}");
            sb.AppendLine($"{Tailers}");
            return sb.ToString();
        }
        public string? Tailers { get => "{5:{CHK:ABCDEF123456}}"; }
    }
}
