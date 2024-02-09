﻿/*using System;
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

namespace SQLServerConnector
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string configFilePath = @"C:\Users\Raslen\OneDrive\Bureau\devops\dev\MqttConnector\SQLServerConnector\config.json";
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
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to SQL Server: " + ex.Message);
            }
        }

        static void StartZeroMQSubscriber(string connectionString, ZeroMQConfig zeroMQConfig)
        {
            using (var subscriber = new SubscriberSocket())
            {
                subscriber.Connect(zeroMQConfig.ServerAddress);
                subscriber.Subscribe("");

                Console.WriteLine("ZeroMQ subscriber started and connected.");

                while (true)
                {
                    string message = subscriber.ReceiveFrameString();

                    string[] messageParts = message.Split('|');
                    if (messageParts.Length == 3)
                    {
                        string payload = messageParts[1];
                        Console.WriteLine("Received message from ZeroMQ: " + payload);

                        if (DateTime.TryParse(messageParts[2], out DateTime timestamp))
                        {
                            InsertMessageIntoDatabase(connectionString, payload, timestamp);
                        }
                        else
                        {
                            Console.WriteLine("Invalid timestamp format.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid message format.");
                    }
                }
            }
        }



        static string GetConnectionString(SqlServerConfig sqlConfig)
        {
            return $"Data Source={sqlConfig.DataSource};Initial Catalog={sqlConfig.InitialCatalog};Integrated Security={sqlConfig.IntegratedSecurity};TrustServerCertificate={sqlConfig.TrustServerCertificate}";
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
                throw;
            }
        }

        static void InsertMessageIntoDatabase(string connectionString, string payload, DateTime receivedTime)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO Messages (Topic, Payload, ReceivedTime) VALUES (@topic, @payload, @receivedTime)";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@topic", "topic");
                        command.Parameters.AddWithValue("@payload", payload);
                        command.Parameters.AddWithValue("@receivedTime", receivedTime);
                        command.ExecuteNonQuery();
                        Console.WriteLine("Message inserted into SQL Server database.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inserting message into SQL Server database: " + ex.Message);
            }
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
        public bool IntegratedSecurity { get; set; }
        public bool TrustServerCertificate { get; set; }
    }

    class ZeroMQConfig
    {
        public string ServerAddress { get; set; }
    }



