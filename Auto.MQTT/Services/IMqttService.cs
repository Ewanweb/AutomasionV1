using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto.MQTT.Services
{
    public interface IMqttService
    {
        Task ConnectAsync();
        Task<bool> IsConnected();
        Task PublishAsync(string topic, string message);
        Task TemperatureAndHumidity(CancellationToken stoppingToken);
    }
}
