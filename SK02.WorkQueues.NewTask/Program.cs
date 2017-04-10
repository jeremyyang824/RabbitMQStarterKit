using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;

namespace SK02.WorkQueues.NewTask
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
                    queue: "task_queue",
                    durable: true,      //持久化队列
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var message = getMessage(args);
                var body = Encoding.UTF8.GetBytes(message);

                IBasicProperties properties = channel.CreateBasicProperties();
                properties.Persistent = true;   //持久化消息（Channel的DeliveryMode将置为2）

                channel.BasicPublish(
                    exchange: "",
                    routingKey: "task_queue",
                    basicProperties: properties,
                    body: body);

                Console.WriteLine(" SK02.WorkQueues.NewTask: {0}", message);
            }

            Console.WriteLine(" Press [enter] to exit...");
            Console.ReadLine();
        }

        private static string getMessage(string[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
        }
    }
}
