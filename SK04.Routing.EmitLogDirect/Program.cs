using System;
using System.Linq;
using System.Text;
using RabbitMQ.Client;

namespace SK04.Routing.EmitLogDirect
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
                    exchange: "direct_logs",
                    type: "direct");

                var severity = getSeverity(args);
                var message = getMessage(args);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(
                    exchange: "direct_logs",
                    routingKey: severity,   //以LogSeverity作为routingKey
                    basicProperties: null,
                    body: body);

                Console.WriteLine(" SK04.Routing.EmitLogDirect: '{0}':'{1}'", severity, message);
            }

            Console.WriteLine(" Press [enter] to exit...");
            Console.ReadLine();
        }

        private static string getSeverity(string[] args)
        {
            return (args.Length > 0) ? args[0] : "info";
        }

        private static string getMessage(string[] args)
        {
            return (args.Length > 1) ? string.Join(" ", args.Skip(1).ToArray()) : "Hello World!";
        }
    }
}
