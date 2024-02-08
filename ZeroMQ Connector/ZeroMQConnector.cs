using System;
using NetMQ;
using NetMQ.Sockets;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string connectionString = @"Data Source=LAPTOP-TQUNE01B\SQLEXPRESS;Initial Catalog=storage;Integrated Security=True;TrustServerCertificate=true;";

        var serverThread = new System.Threading.Thread(() => StartZeroMQServer(connectionString));
        serverThread.Start();

        StartZeroMQClient();

        await Task.Delay(-1);

    }

    static void StartZeroMQServer(string connectionString)
    {
        using (var server = new ResponseSocket("@tcp://*:5556"))
        {
            Console.WriteLine("ZeroMQ server started and listening on port 5556");

            while (true)
            {
                string message = server.ReceiveFrameString();
                Console.WriteLine("Received message from MQTT: " + message);

                string[] messageParts = message.Split('|');
                if (messageParts.Length == 3)
                {
                    string topic = messageParts[0];
                    string payload = messageParts[1];
                    DateTime receivedTime;
                    if (DateTime.TryParse(messageParts[2], out receivedTime))
                    {
                        StoreMessageInDatabase(connectionString, topic, payload, receivedTime);
                        server.SendFrame("Acknowledged");
                    }
                    else
                    {
                        Console.WriteLine("Invalid received time format.");
                        server.SendFrame("Error: Invalid received time format.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid message format.");
                    server.SendFrame("Error: Invalid message format.");
                }
            }
        }
    }


    static void StartZeroMQClient()
{
    using (var client = new RequestSocket(">tcp://localhost:5556"))
    {
        while (true)
        {
            string messageFromMQTT = "topic|Message from MQTT|" + DateTime.Now;

            client.SendFrame(messageFromMQTT);

            string acknowledgment = client.ReceiveFrameString();
            Console.WriteLine("Acknowledgment from ZeroMQ server: " + acknowledgment);

            Thread.Sleep(5000);
        }
    }
}


    static void StoreMessageInDatabase(string connectionString, string topic, string payload, DateTime receivedTime)
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
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error storing message in database: " + ex.Message);
        }
    }
}




