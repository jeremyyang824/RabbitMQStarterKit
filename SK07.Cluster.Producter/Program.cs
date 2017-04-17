using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;

namespace SK07.Cluster.Producter
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5670 };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                

                var message = getMessage(args);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(
                    exchange: "cluster_exchange_test",
                    routingKey: "cluster_rk",
                    body: body);

                Console.WriteLine(" SK07.Cluster.Producter: {0}", message);
            }
        }

        private static string getMessage(string[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
        }
    }
}
