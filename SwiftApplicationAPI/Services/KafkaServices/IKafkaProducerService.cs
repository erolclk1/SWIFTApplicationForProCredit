namespace SwiftApplicationAPI.Services.KafkaServices
{
    public interface IKafkaProducerService
    {
        public Task<int> SendMessageAsync(string topic, string message);
    }
}
