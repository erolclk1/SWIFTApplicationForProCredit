using MediatR;
using SwiftApplicationAPI.Controllers;
using SwiftApplicationAPI.Models;
using SwiftApplicationAPI.Models.ParseMTModels;
using SwiftApplicationAPI.Services.AuthenticationServices;
using SwiftApplicationAPI.Services.KafkaServices;
using System.Text;
using System.Windows.Input;

namespace SwiftApplicationAPI.Queries.GetSwiftMessage
{
    public record MT103MessageGeneratorCommand(MT103ModelDTO MT103model) : IRequest<int>;

    public class MT103MessageGeneratorCommandHandler : IRequestHandler<MT103MessageGeneratorCommand, int>
    {
        private readonly ILogger<MT103MessageGeneratorCommandHandler> logger;
        private readonly IKafkaProducerService kafkaProducerService;
        private readonly IHttpContextTokenAccessorService httpContextTokenAccessor;

        public MT103MessageGeneratorCommandHandler(ILogger<MT103MessageGeneratorCommandHandler> logger,
            IKafkaProducerService kafkaProducerService,
            IHttpContextTokenAccessorService httpContextTokenAccessor)
        {
            this.logger = logger;
            this.kafkaProducerService = kafkaProducerService;
            this.httpContextTokenAccessor = httpContextTokenAccessor;
        }
        public async Task<int> Handle(MT103MessageGeneratorCommand command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Generating the message");
            await httpContextTokenAccessor.SetToken();
            var fileName = $"MT103_{DateTime.UtcNow:yyyyMMdd_HHmmss}.swift";
            var baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "messageFilesMT103");
            Directory.CreateDirectory(baseDirectory);
            var filePath = Path.Combine(baseDirectory, fileName);
            File.WriteAllText(filePath, command.MT103model.ToString());
            logger.LogInformation($"Saved in {filePath} and the name of the file is messageFiles");
            logger.LogInformation("Sending message through Kafka");
            await kafkaProducerService.SendMessageAsync("swift-messages", filePath);
            return 1;
        }
    }
}
