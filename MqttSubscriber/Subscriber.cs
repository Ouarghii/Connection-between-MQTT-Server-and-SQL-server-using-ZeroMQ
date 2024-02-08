// See https://aka.ms/new-console-template for more information
using MQTTnet.Client;
using MQTTnet;
using System;
using MQTTnet.Client.Options;
using System.Text;

namespace MqttSubscriber
{
    class Subscriber
    {
        static async Task Main(string[] args)
        {
            var mqttFactory = new MqttFactory();
            var client = mqttFactory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer("test.mosquitto.org", 1883)
                .WithCleanSession()
                .Build();
            client.UseConnectedHandler(async e =>
            {
                Console.WriteLine("Connected to Brocker Successfully");
                var topicFilter = new TopicFilterBuilder()
                  .WithTopic("RaslenOuarghi")
                  .Build();
                await client.SubscribeAsync(topicFilter);
            });

            client.UseDisconnectedHandler(e =>
            {
                Console.WriteLine("Disconnected from the broker successfully");
            });

            client.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine($"Received Message - {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            });
            await client.ConnectAsync(options);
            

            Console.ReadLine();
            await client.DisconnectAsync();
        }
    }
}
