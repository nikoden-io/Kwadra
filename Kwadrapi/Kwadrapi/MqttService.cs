using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MQTTnet;
using MQTTnet.Client;
using JsonException = Newtonsoft.Json.JsonException;

namespace Kwadrapi;

public class SensorData
{
    [JsonPropertyName("temperature")] public double Temperature { get; set; }
    [JsonPropertyName("humidity")] public double Humidity { get; set; }
}

public class MqttService
{
    private readonly IConfiguration _configuration;
    private readonly IMqttClient _mqttClient;

    public MqttService(IConfiguration configuration)
    {
        _configuration = configuration;
        var mqttFactory = new MqttFactory();
        _mqttClient = mqttFactory.CreateMqttClient();
    }

    public async Task ConnectAsync()
    {
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(_configuration["MQTT:BrokerUrl"], int.Parse(_configuration["MQTT:BrokerPort"]))
            .WithCredentials(_configuration["MQTT:BrokerUsername"], _configuration["MQTT:BrokerPassword"])
            .WithCleanSession()
            .Build();

        var connectResult = await _mqttClient.ConnectAsync(options);

        if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
        {
            Console.WriteLine("Connected to MQTT broker successfully in class.");
            await SubscribeToTopicAsync(_configuration["MQTT:IotTopic"]);
        }
    }

    private async Task SubscribeToTopicAsync(string topic)
    {
        await _mqttClient.SubscribeAsync(topic);
        _mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            var message = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
            Console.WriteLine($"Received message: {message}");

            try
            {
                var sensorData = JsonSerializer.Deserialize<SensorData>(message);
                if (sensorData != null)
                    // Now you can work with sensorData as a C# object
                    Console.WriteLine($"Temperature: {sensorData.Temperature}, Humidity: {sensorData.Humidity}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing MQTT message: {ex.Message}");
            }

            return Task.CompletedTask;
        };
    }
}