using MQTTnet;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Auto.MQTT.Services
{
    public class MqttService : IMqttService
    {
        private readonly IMqttClient _client;
        private readonly MqttClientOptions _options;

        // Channel را به عنوان یک فیلد خصوصی کلاس نگه می‌داریم تا در کل برنامه قابل دسترسی باشد
        private readonly Channel<(string Topic, string Payload)> _messageChannel;

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
                .WithCredentials("mahan", "Mahan1384")
                .WithTlsOptions(new MqttClientTlsOptions()
                {
                    UseTls = true,
                    AllowUntrustedCertificates = false,
                    IgnoreCertificateChainErrors = false,
                    IgnoreCertificateRevocationErrors = false,
                    CertificateValidationHandler = context => true
                })
                .WithCleanSession()
                .Build();

            // یک Channel برای نگهداری تمام پیام‌های دریافتی ایجاد می‌کنیم
            _messageChannel = Channel.CreateUnbounded<(string Topic, string Payload)>();

            // فقط یک Event Handler کلی برای دریافت همه پیام‌ها تنظیم می‌کنیم
            _client.ApplicationMessageReceivedAsync += OnApplicationMessageReceived;
        }

        private Task OnApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            var topic = e.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

            // پیام دریافتی را به همراه تاپیک آن در Channel می‌نویسیم
            _messageChannel.Writer.TryWrite((topic, payload));

            return Task.CompletedTask;
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

        public async IAsyncEnumerable<(double Temperature, double Humidity)> TemperatureAndHumidity(
            string topic,
            [EnumeratorCancellation] CancellationToken stoppingToken)
        {
            if (!_client.IsConnected)
            {
                Console.WriteLine("MQTT client is not connected. Cannot subscribe.");
                yield break;
            }

            // ابتدا به تاپیک مورد نظر اشتراک می‌شویم
            await _client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());

            Console.WriteLine($"Subscribed to topic: {topic}");

            // حالا از Channel می‌خوانیم
            await foreach (var (receivedTopic, payload) in _messageChannel.Reader.ReadAllAsync(stoppingToken))
            {
                // از عبارت باقاعده برای پیدا کردن تمام اعداد در رشته استفاده می‌کنیم
                var matches = Regex.Matches(payload, @"-?\d+(\.\d+)?");

                // اگر دقیقا دو عدد پیدا شد، آن‌ها را پردازش می‌کنیم
                if (matches.Count == 2 &&
                    double.TryParse(matches[0].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double temperature) &&
                    double.TryParse(matches[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double humidity))
                {
                    Console.WriteLine($"پیام دریافت شد! تاپیک: {topic}، دما: {temperature}، رطوبت: {humidity}");
                    yield return (temperature, humidity);
                }
                else
                {
                    Console.WriteLine($"خطا در تجزیه پیام: '{payload}'");
                }

            }

        }
    }
}