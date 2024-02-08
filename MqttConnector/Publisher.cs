using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using NetMQ.Sockets;
using System.Threading;

namespace MqttConnector
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("localhost", 1883) 
                .Build();

            await mqttClient.ConnectAsync(options);

            if (mqttClient.IsConnected)
            {
                Console.WriteLine("Connected to MQTT broker.");
            }
            else
            {
                Console.WriteLine("Failed to connect to MQTT broker.");
                return; 
            }

            while (true)
            {
                string message = "Hello from MQTT Publisher";
                DateTime receivedTime = DateTime.Now; 
                string formattedMessage = $"topic|{message}|{receivedTime}";

                var mqttMessage = new MqttApplicationMessageBuilder()
                    .WithTopic("topic")
                    .WithPayload(formattedMessage)
                    .Build();

                await mqttClient.PublishAsync(mqttMessage);
                Console.WriteLine("Message published: " + formattedMessage);

                Thread.Sleep(5000); 
            }
        }

    }
}






