using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SK07.Cluster.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    //以HAProxy负载均衡入口作为连接参数
                    var factory = new ConnectionFactory() { HostName = "localhost", Port = 5670 };
                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        channel.ExchangeDeclare(
                            exchange: "cluster_exchange_test",
                            type: "direct",
                            autoDelete: false);

                        channel.QueueDeclare(
                            queue: "cluster_queue_test",
                            autoDelete: false);

                        channel.QueueBind(
                            queue: "cluster_queue_test",
                            exchange: "cluster_exchange_test",
                            routingKey: "cluster_rk");

                        Console.WriteLine(" [*] Waiting for messages...");

                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);
                            Console.WriteLine(" SK07.Cluster.Consumer: {0}", message);
                        };

                        channel.BasicConsume(
                            queue: "cluster_queue_test",
                            noAck: false,
                            consumer: consumer);

                        Console.WriteLine(" Press [enter] to exit...");
                        Console.ReadLine();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format(" [ERROR] {0}", ex.Message));
                }
            } //end while
        }
    }
}
