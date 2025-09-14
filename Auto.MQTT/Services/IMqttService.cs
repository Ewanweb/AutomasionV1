using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Auto.MQTT.Services
{
    public interface IMqttService
    {
        Task ConnectAsync();
        Task<bool> IsConnected();
        Task PublishAsync(string topic, string message);
        IAsyncEnumerable<(double Temperature, double Humidity)> TemperatureAndHumidity(string topic,[EnumeratorCancellation] CancellationToken stoppingToken);
    }
}
