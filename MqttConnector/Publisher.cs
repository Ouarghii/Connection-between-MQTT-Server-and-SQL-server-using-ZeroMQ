/*using System;
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
}*/
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using NetMQ;
using NetMQ.Sockets;
using System.Threading;

namespace MqttConnector
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string configFilePath = @"C:\Users\Raslen\OneDrive\Bureau\devops\dev\MqttConnector\MqttConnector\config.json";

            // Load configuration from JSON file
            var config = LoadConfiguration(configFilePath);

            // Create MQTT client
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(config.MqttBroker.Host, config.MqttBroker.Port)
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

            // Create ZeroMQ publisher socket
            using (var zeroMqPublisher = new PublisherSocket())
            {
                zeroMqPublisher.Bind($"tcp://*:{config.ZeroMqPort}");
                 
                while (true)
                {
                    string queryResult = "Data from Blockycollection";

                    string messageForSQL = $"topic|{queryResult}|{DateTime.Now}";

                    zeroMqPublisher.SendFrame(messageForSQL);

                    string messageForMQTT = $"topic|{queryResult}";
                    var mqttMessage = new MqttApplicationMessageBuilder()
                        .WithTopic("topic")
                        .WithPayload(messageForMQTT)
                        .Build();

                    await mqttClient.PublishAsync(mqttMessage);
                    Console.WriteLine("Message published: " + messageForMQTT);

                    Thread.Sleep(5000);
                }
            }
        }

        static Configuration LoadConfiguration(string configFile)
        {
            try
            {
                var json = File.ReadAllText(configFile);
                return JsonSerializer.Deserialize<Configuration>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading configuration: {ex.Message}");
                throw;
            }
        }
    }

    class Configuration
    {
        public MqttBrokerConfig MqttBroker { get; set; }
        public int ZeroMqPort { get; set; }
    }

    class MqttBrokerConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
    }
}








