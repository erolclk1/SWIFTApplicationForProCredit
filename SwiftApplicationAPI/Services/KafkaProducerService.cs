using Confluent.Kafka;

namespace SwiftApplicationAPI.Services
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly ILogger<KafkaProducerService> _logger;
        private readonly IProducer<Null, string> _producer;
        public KafkaProducerService(ILogger<KafkaProducerService> logger)
        {
            _logger = logger;

            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092"
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();
        }
        public async Task<int> SendMessageAsync(string topic, string message)
        {

            try
            {
                var deliveryResult = await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
                _logger.LogInformation($"Kafka message sent to {deliveryResult.TopicPartitionOffset}: {message}");
                return 1;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending Kafka message: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }
    }
}
