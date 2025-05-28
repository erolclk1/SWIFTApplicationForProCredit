namespace SwiftApplicationAPI.Models.CommandDtos
{
    public class RegisterUserDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string CountryCode { get; set; }
        public string Password { get; set; }
    }
}
