namespace SwiftApplicationAPI.Models.Users
{
    public class BankModel
    {
        public int AccountId { get; set; }
        public int UserId { get; set; }
        public string IBANOrBIC { get; set; }
        public decimal Balance { get; set; } = 0;
        public string Currency { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserModel User { get; set; }
    }
}
