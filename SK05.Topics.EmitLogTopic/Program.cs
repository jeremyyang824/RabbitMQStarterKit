using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;

namespace SK05.Topics.EmitLogTopic
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(
                    exchange: "topic_logs",
                    type: "topic");

                var routingKey = getRoutingKey(args);
                var message = getMessage(args);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(
                    exchange: "topic_logs",
                    routingKey: routingKey,
                    basicProperties: null,
                    body: body);

                Console.WriteLine(" SK05.Topics.EmitLogTopic: '{0}':'{1}'", routingKey, message);
            }

            Console.WriteLine(" Press [enter] to exit...");
            Console.ReadLine();
        }

        private static string getRoutingKey(string[] args)
        {
            return (args.Length > 0) ? args[0] : "anonymous.info";
        }

        private static string getMessage(string[] args)
        {
            return (args.Length > 1) ? string.Join(" ", args.Skip(1).ToArray()) : "Hello World!";
        }
    }
}
