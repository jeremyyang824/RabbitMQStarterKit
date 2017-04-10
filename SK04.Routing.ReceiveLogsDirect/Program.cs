using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SK04.Routing.ReceiveLogsDirect
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

                //由Consumer创建临时队列
                var queueName = channel.QueueDeclare().QueueName;

                if (args.Length < 1)
                {
                    Console.Error.WriteLine("Usage: {0} [info] [warning] [error]", Environment.GetCommandLineArgs()[0]);
                    Console.WriteLine(" Press [enter] to exit...");
                    Console.ReadLine();
                    return;
                }

                //每种LogSeverity创建一个binding，绑定到临时队列上
                foreach (var severity in args)
                {
                    channel.QueueBind(
                        queue: queueName, 
                        exchange: "direct_logs", 
                        routingKey: severity);  //以LogSeverity作为routingKey
                }

                Console.WriteLine(" [*] Waiting for messages...");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;
                    Console.WriteLine("SK04.Routing.ReceiveLogsDirect: '{0}':'{1}'", routingKey, message);
                };

                channel.BasicConsume(
                    queue: queueName, 
                    noAck: true, 
                    consumer: consumer);

                Console.WriteLine(" Press [enter] to exit...");
                Console.ReadLine();
            }
        }
    }
}
