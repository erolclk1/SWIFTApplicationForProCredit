namespace SwiftApplicationAPI.Models.Users
{
    public class UserModel
    {   
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CountryCode { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
