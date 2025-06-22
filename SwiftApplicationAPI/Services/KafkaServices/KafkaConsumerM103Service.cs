
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using SwiftApplicationAPI.Models.ParseMTModels;
using SwiftApplicationAPI.Services.AuthenticationServices;
using SwiftApplicationAPI.Services.Currency;
using SwiftApplicationAPI.Services.NotifactionService;
using SwiftApplicationAPI.Services.RepositoryQueries;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SwiftApplicationAPI.Services.KafkaServices
{
    public class KafkaConsumerM103Service : BackgroundService
    {
        private readonly ILogger<KafkaConsumerM103Service> logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ICurrencyConverterService converter;
        private readonly IHttpContextTokenAccessorService httpContextTokenAccessor;
        private readonly IUpdateAmountRepository updateAmountRepository;
        private readonly INotifactionService notifactionService;

        public KafkaConsumerM103Service(
            ILogger<KafkaConsumerM103Service> logger,
            IHttpClientFactory httpClientFactory,
            ICurrencyConverterService converter,
            IHttpContextTokenAccessorService httpContextTokenAccessor,
            IUpdateAmountRepository updateAmountRepository,
            INotifactionService notifactionService)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.converter = converter;
            this.httpContextTokenAccessor = httpContextTokenAccessor;
            this.updateAmountRepository = updateAmountRepository;
            this.notifactionService = notifactionService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(5000, stoppingToken);

            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "swift-parser-consumer",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe("swift-messages");
            while (!stoppingToken.IsCancellationRequested)
            {
                    var mt103Model = await consumerGetParsedMessage(consumer, stoppingToken);
                try
                {
                    var convertedCurrencyAmount = await converter.extractMoneyAndConvert(mt103Model);
                    convertedCurrencyAmount = Math.Round(convertedCurrencyAmount, 2);
                    try
                    {
                        var isTransactionSuccefful = await updateAmountRepository.transferTransaction(mt103Model, convertedCurrencyAmount);
                        if (isTransactionSuccefful.Success)
                        {
                            logger.LogInformation(isTransactionSuccefful.Message);
                            await notifactionService.sendStatusMT799Message(mt103Model, isTransactionSuccefful.Message, isTransactionSuccefful.Success);
                        }
                        else
                        {
                            logger.LogInformation($"Someting went wrong with the transaction :{isTransactionSuccefful.Message}");
                            await notifactionService.sendStatusMT799Message(mt103Model, isTransactionSuccefful.Message, isTransactionSuccefful.Success);
                        }
                    }
                    catch (Exception ex)
                    {
                        await notifactionService.sendStatusMT799Message(mt103Model, ex.Message, false);
                    }
                }
                catch (Exception ex) 
                {
                    await notifactionService.sendStatusMT799Message(mt103Model, ex.Message, false);
                }
            }
        }

        private async Task<MT103Model> consumerGetParsedMessage(IConsumer<Ignore, string> consumer, CancellationToken stoppingToken)
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
                        var response = await client.PostAsync("https://localhost:7220/Swift/GetSwift103Message", content);
                        var resultModel = await response.Content.ReadFromJsonAsync<MT103Model>(stoppingToken);
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
