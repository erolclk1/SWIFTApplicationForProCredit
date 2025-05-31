using MediatR;
using SwiftApplicationAPI.Controllers;
using SwiftApplicationAPI.Models;
using SwiftApplicationAPI.Models.ParseMTModels;
using SwiftApplicationAPI.Services;
using SwiftApplicationAPI.Services.KafkaServices;
using System.Text;
using System.Windows.Input;

namespace SwiftApplicationAPI.Queries.GetSwiftMessage
{
    public record MT799MessageGeneratorCommand(MT799ModelDTO MT799model) : IRequest<int>;

    public class MT799MessageGeneratorCommandHandler : IRequestHandler<MT799MessageGeneratorCommand, int>
    {
        private readonly ILogger<MT799MessageGeneratorCommandHandler> logger;
        private readonly IKafkaProducerService kafkaProducerService;

        public MT799MessageGeneratorCommandHandler(
            ILogger<MT799MessageGeneratorCommandHandler> logger,
            IKafkaProducerService kafkaProducerService)
        {
            this.logger = logger;
            this.kafkaProducerService = kafkaProducerService;
        }
        public async Task<int> Handle(MT799MessageGeneratorCommand command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Generating the message");
            var fileName = $"MT103_{DateTime.UtcNow:yyyyMMdd_HHmmss}.swift";
            var baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "messageFilesMT799");
            Directory.CreateDirectory(baseDirectory);
            var filePath = Path.Combine(baseDirectory, fileName);
            File.WriteAllText(filePath, command.MT799model.ToString());
            logger.LogInformation($"Saved in {filePath} and the name of the file is messageFiles");
            await kafkaProducerService.SendMessageAsync("mt799-swift-message", filePath);
            return 1;
        }
    }
}
