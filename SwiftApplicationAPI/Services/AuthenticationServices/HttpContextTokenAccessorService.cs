namespace SwiftApplicationAPI.Services.AuthenticationServices
{
    public class HttpContextTokenAccessorService : IHttpContextTokenAccessorService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public string jwtToken { get; set; }

        public HttpContextTokenAccessorService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<string?> SetToken()
        {
            return jwtToken = httpContextAccessor.HttpContext?
                .Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");
        }
        public async Task<string?> GetToken()
        { 
        return jwtToken;
        }
    }
}
