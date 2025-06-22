namespace SwiftApplicationAPI.Models.CommandDtos
{
    public class AccountInfoDTO
    { 
            public string UserName { get; set; }
            public string Email { get; set; }
            public string CountryCode { get; set; }
            public decimal Balance { get; set; }
            public string IbanOrBic { get; set; }
            public string Currency { get; set; }
    }
}
