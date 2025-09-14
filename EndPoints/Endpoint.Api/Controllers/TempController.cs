using Auto.MQTT;
using Auto.MQTT.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Endpoint.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TempController : ControllerBase
    {
        private readonly IMqttService _mqttService;

        public TempController(IMqttService mqttService)
        {
            _mqttService = mqttService;
        }

        [HttpGet("dht-stream")]
        public async Task GetDhtDataStream(CancellationToken stoppingToken)
        {
            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("X-Accel-Buffering", "no"); // برای Nginx اگر بود

            // استریم دما و رطوبت
            await foreach (var data in _mqttService.TemperatureAndHumidity(Topics.Temp, stoppingToken))
            {
                var json = JsonSerializer.Serialize(new { data.Temperature, data.Humidity });

                // ارسال به کلاینت طبق فرمت SSE
                await Response.WriteAsync($"data: {json}\n\n", stoppingToken);
                await Response.Body.FlushAsync(stoppingToken);
            }
        }
    }
}
