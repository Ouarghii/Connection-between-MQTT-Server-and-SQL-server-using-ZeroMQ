using System;
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

}

