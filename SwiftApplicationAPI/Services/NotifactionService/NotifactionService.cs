using SwiftApplicationAPI.Models.ParseMTModels;
using System.Net.Http.Headers;
using System.Net.Http;
using SwiftApplicationAPI.Services.AuthenticationServices;
using SwiftApplicationAPI.Services.Currency;
using SwiftApplicationAPI.Services.KafkaServices;
using SwiftApplicationAPI.Services.RepositoryQueries;
using SwiftApplicationAPI.Models;
using System.Text.Json;
using System.Text;

namespace SwiftApplicationAPI.Services.NotifactionService
{
    public class NotifactionService : INotifactionService
    {
        private readonly ILogger<NotifactionService> logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IHttpContextTokenAccessorService httpContextTokenAccessor;
        private readonly IUpdateAmountRepository updateAmountRepository;

        public NotifactionService(ILogger<NotifactionService> logger,
            IHttpClientFactory httpClientFactory,
            IHttpContextTokenAccessorService httpContextTokenAccessor,
            IUpdateAmountRepository updateAmountRepository)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.httpContextTokenAccessor = httpContextTokenAccessor;
            this.updateAmountRepository = updateAmountRepository;
        }
        public async Task sendStatusMT799Message(MT103Model mT103Model, string Message, bool Successful)
        {
            var client = httpClientFactory.CreateClient();
            var jwtToken = await httpContextTokenAccessor.GetToken();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            var mt799Model = new MT799ModelDTO
            {
                BasicHeader = mT103Model.BasicHeader,
                ApplicationHeader = mT103Model.ApplicationHeader,
                TransactionReference= mT103Model.TransactionReference,
                Tailers = mT103Model.Tailers,
            };
            if (Successful)
            {
                mt799Model.NarrativeMessage = "Your transaction was succeful and the funds have arrived at the reciever";
            }
            else
            {
                mt799Model.NarrativeMessage = $"Someting went wrong with the transaction here is the message : {Message}";
            }
            var json = JsonSerializer.Serialize(mt799Model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7220/MTSwiftGenerator/SWIFTMT799MessageGenerator", content);
        }
    }
}
