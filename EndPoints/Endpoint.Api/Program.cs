using Auto.Infrastructure;
using Auto.MQTT.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------
// 1️⃣ Add services
// ---------------------------

// CORS برای React Dev Server
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5174")
            .AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Controllers + JSON Enum Converter (camelCase)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        );
    });

// Swagger / OpenAPI
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

// Init Facade / MediatR / Infrastructure / MQTT
ApplicationBootsrtrapper.Init(builder.Services, builder.Configuration);

var app = builder.Build();

// ---------------------------
// 2️⃣ Middleware
// ---------------------------

// ⚠️ ترتیب مهم است:
// 1- HTTPS Redirect
// 2- CORS
// 3- Authorization
app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthorization();

// MapControllers بعد از CORS و Authorization
app.MapControllers();

// Swagger / OpenAPI فقط در Development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// MQTT Connect بعد از Build
var mqttService = app.Services.GetRequiredService<IMqttService>();
await mqttService.ConnectAsync();

// ساده‌ترین endpoint برای تست
app.MapGet("/", () => "MQTT Service Running!");

// ---------------------------
// 3️⃣ Run
// ---------------------------
app.Run();
