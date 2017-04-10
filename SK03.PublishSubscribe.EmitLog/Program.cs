using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;

namespace SK03.PublishSubscribe.EmitLog
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
                    exchange: "logs", 
                    type: "fanout");    //fanout: broadcasts all the messages to all the queues

                var message = getMessage(args);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(
                    exchange: "logs", 
                    routingKey: "",     //发布到fanout交换机（广播），无需routingKey
                    basicProperties: null, 
                    body: body);

                Console.WriteLine(" SK03.PublishSubscribe.EmitLog:{0}", message);
            }
        }

        private static string getMessage(string[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "info: Hello World!");
        }
    }
}
