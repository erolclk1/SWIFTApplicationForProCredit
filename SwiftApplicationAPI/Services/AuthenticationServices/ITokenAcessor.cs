namespace SwiftApplicationAPI.Services.AuthenticationServices
{
    public interface IHttpContextTokenAccessorService
    {
        public Task<string?> SetToken();
        public Task<string?> GetToken();
    }
}
