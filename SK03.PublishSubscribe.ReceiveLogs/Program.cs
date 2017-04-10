using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SK03.PublishSubscribe.ReceiveLogs
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
                    type: "fanout");

                //声明一个: non-durable, exclusive, autodelete queue with a random name.
                //注意该临时队列是在Consumer端声明
                var queueName = channel.QueueDeclare().QueueName;

                //声明一个binding: 绑定临时队列到logs交换机
                channel.QueueBind(
                    queue: queueName, 
                    exchange: "logs", 
                    routingKey: "");    //fanout交换机，无需routingKey

                Console.WriteLine(" [*] Waiting for logs...");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" SK03.PublishSubscribe.ReceiveLogs: {0}", message);
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
