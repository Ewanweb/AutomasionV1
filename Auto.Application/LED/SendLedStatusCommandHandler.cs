using Auto.Application._Shared;
using Auto.MQTT.Services;
using MediatR;

namespace Auto.Application.LED
{
    public class SendLedStatusCommandHandler : IRequestHandler<SendLedStatusCommand, OperationResult>
    {
        private readonly IMqttService _mqttService;

        public SendLedStatusCommandHandler(IMqttService mqtt)
        {
            _mqttService = mqtt;
        }

        public async Task<OperationResult> Handle(SendLedStatusCommand request, CancellationToken cancellationToken)
        {
            string command = request.Command.ToString().ToLower();
            string topic = request.Topic;

            try
            {
                await _mqttService.PublishAsync(topic, command);
                return OperationResult.Success(command);
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
                return OperationResult.Error($"عملیات شکست خورد به کنسول مراجعه کنید");
            }

        }
    }
}
