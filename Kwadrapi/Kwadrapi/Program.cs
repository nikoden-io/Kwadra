using System.Text;
using Kwadrapi.Routing;
using MQTTnet;
using MQTTnet.Client;

var builder = WebApplication.CreateBuilder(args);

var mqttFactory = new MqttFactory();

using var mqttClient = mqttFactory.CreateMqttClient();

var options = new MqttClientOptionsBuilder()
    .WithTcpServer(builder.Configuration["MQTT:BrokerUrl"], int.Parse(builder.Configuration["MQTT:BrokerPort"]!))
    .WithCredentials(builder.Configuration["MQTT:BrokerUsername"]!, builder.Configuration["MQTT:BrokerPassword"]!)
    .WithCleanSession()
    .Build();

var connectResult = await mqttClient.ConnectAsync(options);

if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
{
    Console.WriteLine("Connected to MQTT broker successfully in program.cs.");
    Console.WriteLine(builder.Configuration["MQTT:IotTopic"]);

    await mqttClient.SubscribeAsync(builder.Configuration["MQTT:IotTopic"]);


    mqttClient.ApplicationMessageReceivedAsync += e =>
    {
        Console.WriteLine("Message received from program.cs");
        Console.WriteLine($"Received message: {Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment)}");
        return Task.CompletedTask;
    };
}


// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add controllers
builder.Services.AddControllers(options => { options.Conventions.Add(new VersionRouteConvention()); });


var app = builder.Build();


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