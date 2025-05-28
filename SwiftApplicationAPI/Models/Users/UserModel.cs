namespace SwiftApplicationAPI.Models.Users
{
    public class UserModel
    {   
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string IBANOrBIC { get; set; }
        public string CountryCode { get; set; }
        public decimal Balance { get; set; } = 0;
        public string PasswordHash { get; set; }
        public string Currency { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
