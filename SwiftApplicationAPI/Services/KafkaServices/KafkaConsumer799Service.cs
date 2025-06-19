
using Confluent.Kafka;
using SwiftApplicationAPI.Models.ParseMTModels;
using System.Net.Http.Headers;
using System.Net.Http;
using SwiftApplicationAPI.Services.AuthenticationServices;
using SwiftApplicationAPI.Services.Currency;
using SwiftApplicationAPI.Services.RepositoryQueries;
using Microsoft.AspNetCore.SignalR;
using SwiftApplicationAPI.SignalR;

namespace SwiftApplicationAPI.Services.KafkaServices
{
    public class KafkaConsumer799Service : BackgroundService
    {
        private readonly ILogger<KafkaConsumer799Service> logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IHttpContextTokenAccessorService httpContextTokenAccessor;
        private readonly IUpdateAmountRepository updateAmountRepository;
        private readonly IHubContext<NotificationHub> hubContext;

        public KafkaConsumer799Service(
            ILogger<KafkaConsumer799Service> logger,
            IHttpContextTokenAccessorService httpContextTokenAccessor,
            IHttpClientFactory httpClientFactory,
            IUpdateAmountRepository updateAmountRepository,
            IHubContext<NotificationHub> hubContext)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.httpContextTokenAccessor = httpContextTokenAccessor;
            this.updateAmountRepository = updateAmountRepository;
            this.hubContext = hubContext;
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(5000, stoppingToken);

            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "swift799",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe("mt799-swift-message");
            var mT799Model = await consumerGetParsedMessage(consumer, stoppingToken);
            await hubContext.Clients.All.SendAsync("ReceiveNotification", mT799Model);

        }
        private async Task<MT799Model> consumerGetParsedMessage(IConsumer<Ignore, string> consumer, CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var result = consumer.Consume(stoppingToken);
                    logger.LogInformation($"Consumed message: {result.Value}");

                    var client = httpClientFactory.CreateClient();
                    var jwtToken = await httpContextTokenAccessor.GetToken();
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                    var content = new MultipartFormDataContent();
                    var fileBytes = await File.ReadAllBytesAsync(result.Value);
                    var fileContent = new ByteArrayContent(fileBytes);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    content.Add(fileContent, "swiftInput", Path.GetFileName(result.Value));

                    try
                    {
                        var response = await client.PostAsync("https://localhost:7220/Swift/GetSwift799Message", content);
                        var resultModel = await response.Content.ReadFromJsonAsync<MT799Model>(stoppingToken);
                        return resultModel;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.Message);
                        continue;
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                logger.LogInformation("Kafka consumer stopped");
                await Task.Delay(3000);
            }
            return null;
        }
    }
}
