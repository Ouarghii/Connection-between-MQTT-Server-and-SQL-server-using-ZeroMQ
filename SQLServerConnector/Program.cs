/*using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Data.SqlClient;
using NetMQ;
using NetMQ.Sockets;
using MQTTnet.Client;

namespace SQLServerConnector
{

    class Program
    {
        static async Task Main(string[] args)
        {
            string connectionString = @"Data Source=LAPTOP-TQUNE01B\SQLEXPRESS;Initial Catalog=storage;Integrated Security=True;TrustServerCertificate=true;";

            TestSqlConnection(connectionString);

            StartSqlSubscriber(connectionString);

            Console.ReadLine(); 
        }


        static void TestSqlConnection(string connectionString)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Connected to SQL Server successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to SQL Server: " + ex.Message);
            }
        }


        static void StartSqlSubscriber(string connectionString)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Messages";
                    using (var command = new SqlCommand(sql, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string message = reader["Payload"].ToString(); 
                            Console.WriteLine("Received message from SQL: " + message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving messages from SQL database: " + ex.Message);
            }
        }

    }

}*/
/*using System;
using System.IO;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using NetMQ;
using NetMQ.Sockets;

namespace SQLServerConnector
{
    class Program
    {
        static void Main(string[] args)
        {
            // Connection string to SQL Server
            var connectionString = "Server=LAPTOP-TQUNE01B\\SQLEXPRESS;Database=storage;Trusted_Connection=True;MultipleActiveResultSets=true";

            // Initialize subscriber socket
            using (var subscriber = new SubscriberSocket())
            {
                // Connect to the publisher socket
                subscriber.Connect("tcp://localhost:5556");

                // Subscribe to topics "myTopic" and "MyTopic"
                subscriber.Subscribe("myTopic");
                subscriber.Subscribe("MyTopic");

                Console.WriteLine("Subscriber socket connected. Waiting for messages...");

                // Continuously receive and process messages
                while (true)
                {
                    // Receive message
                    var message = subscriber.ReceiveFrameString();
                    Console.WriteLine($"Received message: {message}");

                    // Parse message and insert into database
                    try
                    {
                        var parts = message.Split('|');
                        if (parts.Length == 3)
                        {
                            var topic = parts[0];
                            var payload = parts[1];
                            var receivedTime = DateTime.Parse(parts[2]);

                            // Insert message into database using raw SQL
                            using (var connection = new SqlConnection(connectionString))
                            {
                                connection.Open();
                                string sql = "INSERT INTO Messages (Topic, Payload, ReceivedTime) VALUES (@topic, @payload, @receivedTime)";
                                using (var command = new SqlCommand(sql, connection))
                                {
                                    command.Parameters.AddWithValue("@topic", topic);
                                    command.Parameters.AddWithValue("@payload", payload);
                                    command.Parameters.AddWithValue("@receivedTime", receivedTime);
                                    int rowsAffected = command.ExecuteNonQuery();
                                    Console.WriteLine($"Inserted {rowsAffected} row(s) into the database.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid message format.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing message: {ex.Message}");
                    }
                }
            }
        }
    }
}*/
/*using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Data.SqlClient;
using NetMQ;
using NetMQ.Sockets;
using MQTTnet.Client;

namespace SQLServerConnector
{

    class Program
    {
        static async Task Main(string[] args)
        {
            string connectionString = @"Data Source=LAPTOP-TQUNE01B\SQLEXPRESS;Initial Catalog=storage;Integrated Security=True;TrustServerCertificate=true;";

            TestSqlConnection(connectionString);

            StartSqlSubscriber(connectionString);

            Console.ReadLine(); 
        }


        static void TestSqlConnection(string connectionString)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Connected to SQL Server successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to SQL Server: " + ex.Message);
            }
        }


        static void StartSqlSubscriber(string connectionString)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Messages";
                    using (var command = new SqlCommand(sql, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string message = reader["Payload"].ToString(); 
                            Console.WriteLine("Received message from SQL: " + message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving messages from SQL database: " + ex.Message);
            }
        }

    }

}*/

using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using NetMQ;
using NetMQ.Sockets;
using Microsoft.Extensions.Logging;

namespace SQLServerConnector
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
                builder.AddFile(@"C:\Users\Raslen\OneDrive\Bureau\devops\dev\MqttConnector\SQLServerConnector\log_file.txt"); // Specify the path to your log file here

                builder.SetMinimumLevel(LogLevel.Information);
            });

            _logger = loggerFactory.CreateLogger<Program>();

            string configFilePath = "config.json";
            Configuration config = LoadConfiguration(configFilePath);

            string connectionString = GetConnectionString(config.SqlServer);

            TestSqlConnection(connectionString);

            StartZeroMQSubscriber(connectionString, config.ZeroMQ);

            Console.ReadLine();
        }

        static void TestSqlConnection(string connectionString)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Connected to SQL Server successfully.");
                    _logger.LogInformation("Connected to SQL Server successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to SQL Server: " + ex.Message);
                _logger.LogError($"Error connecting to SQL Server: {ex.Message}");
            }
        }

        static void StartZeroMQSubscriber(string connectionString, ZeroMQConfig zeroMQConfig)
        {
            using (var subscriber = new SubscriberSocket())
            {
                subscriber.Connect(zeroMQConfig.ServerAddress);
                subscriber.Subscribe("");

                Console.WriteLine("ZeroMQ subscriber started and connected.");
                _logger.LogInformation("ZeroMQ subscriber started and connected.");

                while (true)
                {
                    string message = subscriber.ReceiveFrameString();
                    string[] messageParts = message.Split('|');
                    if (messageParts.Length == 3)
                    {
                        string topic = messageParts[0];
                        string payload = messageParts[1];
                        if (DateTime.TryParse(messageParts[2], out DateTime timestamp))
                        {
                            InsertMessageIntoDatabase(connectionString, topic, payload, timestamp);
                        }
                        else
                        {
                            Console.WriteLine("Invalid timestamp format.");
                            _logger.LogError("Invalid timestamp format.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid message format.");
                        _logger.LogError("Invalid message format.");
                    }
                }
            }
        }

        // static string GetConnectionString(SqlServerConfig sqlConfig)
        // {
        //     return $"Data Source={sqlConfig.DataSource};Initial Catalog={sqlConfig.InitialCatalog};Integrated Security={sqlConfig.IntegratedSecurity};TrustServerCertificate={sqlConfig.TrustServerCertificate}";
        // }
        static string GetConnectionString(SqlServerConfig sqlConfig)
{
    string authentication = sqlConfig.IntegratedSecurity ? "Integrated Security=true;" : $"User Id={sqlConfig.User};Password={sqlConfig.Password};";
    string trustServerCertificate = sqlConfig.TrustServerCertificate ? "TrustServerCertificate=true;" : "";

    return $"Data Source={sqlConfig.DataSource};Initial Catalog={sqlConfig.InitialCatalog};{authentication}{trustServerCertificate}";
}

        static Configuration LoadConfiguration(string configFile)
        {
            try
            {
                string json = File.ReadAllText(configFile);
                return JsonSerializer.Deserialize<Configuration>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading configuration: {ex.Message}");
                _logger.LogError($"Error loading configuration: {ex.Message}");
                throw;
            }
        }

        static void InsertMessageIntoDatabase(string connectionString, string topic, string payload, DateTime receivedTime)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO Messages (Topic, Payload, ReceivedTime) VALUES (@topic, @payload, @receivedTime)";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@topic", topic);
                        command.Parameters.AddWithValue("@payload", payload);
                        command.Parameters.AddWithValue("@receivedTime", receivedTime);
                        int inserted = command.ExecuteNonQuery();
                        Console.WriteLine("Message inserted into SQL Server database.");
                        _logger.LogInformation("Message inserted into SQL Server database.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inserting message into SQL Server database: " + ex.Message);
                _logger.LogError($"Error inserting message into SQL Server database: {ex.Message}");
            }
        }
    }

    class Configuration
    {
        public SqlServerConfig SqlServer { get; set; }
        public ZeroMQConfig ZeroMQ { get; set; }
    }

    class SqlServerConfig
{
    public string DataSource { get; set; }
    public string InitialCatalog { get; set; }
    public string User { get; set; } // Add this property
    public string Password { get; set; } // Add this property
    public bool IntegratedSecurity { get; set; }
    public bool TrustServerCertificate { get; set; }
}


    class ZeroMQConfig
    {
        public string ServerAddress { get; set; }
    }
}






