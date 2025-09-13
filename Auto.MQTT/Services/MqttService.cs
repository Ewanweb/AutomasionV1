using MQTTnet;
using System;
using System.Threading.Tasks;

namespace Auto.MQTT.Services
{
    public class MqttService : IMqttService
    {
        private readonly IMqttClient _client;
        private readonly MqttClientOptions _options;

        public MqttService()
        {
            var factory = new MqttClientFactory();
            _client = factory.CreateMqttClient();

            _client.ConnectedAsync += e =>
            {
                Console.WriteLine("✅ Connected to MQTT broker");
                return Task.CompletedTask;
            };

            _client.DisconnectedAsync += e =>
            {
                Console.WriteLine("❌ Disconnected from MQTT broker");
                return Task.CompletedTask;
            };

            _options = new MqttClientOptionsBuilder()
                .WithClientId("AspDotNetClient")
                .WithTcpServer("0f041548b53642a096cdd35b3ae6854a.s1.eu.hivemq.cloud", 8883)
                .WithCredentials("mahan", "Mahan1384") // یوزرنیم و پسورد
                .WithTlsOptions(new MqttClientTlsOptions()
                {
                    UseTls = true,
                    AllowUntrustedCertificates = false, // اگر certificate معتبره false بذار
                    IgnoreCertificateChainErrors = false,
                    IgnoreCertificateRevocationErrors = false,
                    CertificateValidationHandler = context => true // فقط برای تست، اعتبارسنجی certificate رو رد می‌کنه
                })
                .WithCleanSession()
                .Build();
        }

        public async Task ConnectAsync()
        {
            await _client.ConnectAsync(_options);
        }

        public Task<bool> IsConnected()
        {
            return Task.FromResult(_client.IsConnected);
        }

        public async Task PublishAsync(string topic, string message)
        {
            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(message)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            if (_client.IsConnected)
            {
                await _client.PublishAsync(mqttMessage);
                Console.WriteLine($"Message published to topic {topic}: {message}");
            }
            else
            {
                Console.WriteLine("MQTT client is not connected!");
            }
        }

        public async Task TemperatureAndHumidity(CancellationToken stoppingToken)
        {
            await _client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("home/dht").Build());

            while (!stoppingToken.IsCancellationRequested) 
            {
                await Task.Delay(10000, stoppingToken);
            }
    }
}
