namespace SwiftApplicationAPI.Services
{
    public interface IKafkaProducerService
    {
        public Task<int> SendMessageAsync(string topic, string message);
    }
}
