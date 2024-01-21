
using MQTTnet;
using MQTTnet.Client;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Kwadrapi;

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
            Console.WriteLine("Message received in class.");
            Console.WriteLine($"Received message: {Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment)}");
            return Task.CompletedTask;
        };
    }
}
