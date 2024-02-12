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
/*using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using NetMQ;
using NetMQ.Sockets;

namespace MqttConnector
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string configFilePath = @"C:\Users\Raslen\OneDrive\Bureau\devops\dev\MqttConnector\MqttConnector\config.json";

            var config = LoadConfiguration(configFilePath);

            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("test.mosquitto.org", 1883) 
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

            using (var zeroMqPublisher = new PublisherSocket())
            {
                zeroMqPublisher.Bind($"tcp://*:{config.ZeroMqPort}");

                // Subscribe to the desired topics
                await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("myTopic").Build());
                await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("MyTopic").Build());

                mqttClient.UseApplicationMessageReceivedHandler(e =>
                {
                    string topic = e.ApplicationMessage.Topic;
                    string payload = e.ApplicationMessage.Payload != null ? Encoding.UTF8.GetString(e.ApplicationMessage.Payload) : string.Empty;
                    string receivedTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                    string message = $"{topic}|{payload}|{receivedTime}";
                    Console.WriteLine(message);
                    zeroMqPublisher.SendFrame(message);

                    
                });

                while (true)
                {
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
        public int ZeroMqPort { get; set; }
    }
}*/


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
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using NetMQ;
using NetMQ.Sockets;

namespace MqttConnector
{
    class Program
    {
        private static ILogger<Program> _logger;

        static async Task Main(string[] args)
        {
            // Setup logging
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddFile(@"C:\Users\Raslen\OneDrive\Bureau\devops\dev\MqttConnector\MqttConnector\log_file.txt"); // Specify the path to your log file here

                builder.SetMinimumLevel(LogLevel.Information);
            });

            _logger = loggerFactory.CreateLogger<Program>();

            string configFilePath = @"C:\Users\Raslen\OneDrive\Bureau\devops\dev\MqttConnector\MqttConnector\config.json";

            var config = LoadConfiguration(configFilePath);

            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(config.MqttBroker.Host, config.MqttBroker.Port)
                .Build();

            try
            {
                await mqttClient.ConnectAsync(options);

                if (mqttClient.IsConnected)
                {
                    Console.WriteLine("Connected to MQTT broker.");
                    _logger.LogInformation("Connected to MQTT broker.");
                }
                else
                {
                    Console.WriteLine("Failed to connect to MQTT broker.");
                    _logger.LogError("Failed to connect to MQTT broker.");
                    return;
                }

                using (var zeroMqPublisher = new PublisherSocket())
                {
                    zeroMqPublisher.Bind($"tcp://*:{config.ZeroMqPort}");

                    await SubscribeToTopics(mqttClient, "rasleeen");

                    mqttClient.UseApplicationMessageReceivedHandler(e =>
                    {
                        string topic = e.ApplicationMessage.Topic;
                        string payload = e.ApplicationMessage.Payload != null ? Encoding.UTF8.GetString(e.ApplicationMessage.Payload) : string.Empty;
                        string receivedTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                        Console.WriteLine($"Received message - Topic: {topic}, Payload: {payload}, Received Time: {receivedTime}");
                        _logger.LogInformation($"Received message - Topic: {topic}, Payload: {payload}, Received Time: {receivedTime}");

                        ReceiveAndSendMessages(zeroMqPublisher, mqttClient, topic, payload);
                    });

                    while (true)
                    {
                        await Task.Delay(10000);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                _logger.LogError($"An error occurred: {ex.Message}");
            }
        }

        static async Task SubscribeToTopics(IMqttClient mqttClient, params string[] topics)
        {
            foreach (var topic in topics)
            {
                await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic(topic).Build());
                Console.WriteLine($"Subscribed to topic: {topic}");
                _logger.LogInformation($"Subscribed to topic: {topic}");
            }
        }

        static void ReceiveAndSendMessages(PublisherSocket zeroMqPublisher, IMqttClient mqttClient, string receivedTopic, string receivedPayload)
        {
            string receivedTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            string messageForSQL = $"{receivedTopic}|{receivedPayload}|{receivedTime}";

            zeroMqPublisher.SendFrame(messageForSQL);

            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(receivedTopic)
                .WithPayload(receivedPayload)
                .Build();

            mqttClient.PublishAsync(mqttMessage);
            Console.WriteLine("Message published: " + receivedPayload);
            _logger.LogInformation("Message published: " + receivedPayload);
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
                _logger.LogError($"Error loading configuration: {ex.Message}");
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






















