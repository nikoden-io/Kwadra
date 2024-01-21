using Kwadrapi;
using Kwadrapi.Routing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<MqttService>();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add controllers
builder.Services.AddControllers(options => { options.Conventions.Add(new VersionRouteConvention()); });


var app = builder.Build();

// Connect to MQTT broker
var mqttService = app.Services.GetRequiredService<MqttService>();
await mqttService.ConnectAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map controllers
app.MapControllers();

app.Run();