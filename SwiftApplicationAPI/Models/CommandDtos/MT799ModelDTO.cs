using System.Text;

namespace SwiftApplicationAPI.Models
{

    public class MT799ModelDTO
    {
        private Random random = new Random();
        private string _basicHeader;
        private string _applicationHeader;
        private string _userHeader;
        private string _textHeader;

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
        public string TransactionReference { get; set; } //For block 4 20: ..
        public string NarrativeMessage { get; set; } //For block 4 79:...
        public string Text
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine("{4:");
                sb.AppendLine($":20:{TransactionReference}");
                sb.AppendLine($":79:{NarrativeMessage}");
                sb.AppendLine("-}");
                return sb.ToString();
            }
        }   // block 3
        public string? Tailers { get => "{5:{CHK:ABCDEF123456}}"; } //block 5

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
    }
}
