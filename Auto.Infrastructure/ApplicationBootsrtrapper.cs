using Auto.Application.LED;
using Auto.Facade.LED;
using Auto.MQTT.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto.Infrastructure
{
    public static class ApplicationBootsrtrapper
    {
        public static IServiceCollection Init(IServiceCollection services, IConfiguration config)
        {


            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(SendLedStatusCommand).Assembly);
            });

            services.AddSingleton<IMqttService, MqttService>();
            services.AddScoped<ILedFacade, LedFacade>();
            return services;
        }
    }
}
