using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SK06.RPC.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //RPC请求队列（非匿名）
                channel.QueueDeclare(
                    queue: "rpc_queue",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                channel.BasicQos(0, 1, false);

                //RPC请求接收
                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(
                    queue: "rpc_queue",
                    noAck: false,
                    consumer: consumer);

                Console.WriteLine(" SK06.RPC.Server Awaiting RPC requests...");

                consumer.Received += (model, ea) =>
                {
                    string response = null;

                    //请求消息
                    var body = ea.Body;
                    var props = ea.BasicProperties;
                    try
                    {
                        var message = Encoding.UTF8.GetString(body);
                        int n = int.Parse(message);
                        Console.WriteLine(" SK06.RPC.Server Received: fib({0})", message);
                        response = fib(n).ToString();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" SK06.RPC.Server Error: " + e.Message);
                        response = "";
                    }

                    //组织响应
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;     //关联ID（来自Request）

                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    channel.BasicPublish(
                        exchange: "", 
                        routingKey: props.ReplyTo,      //以请求消息的ReplyTo（即响应消息的匿名队列名）作为响应消息的RoutingKey
                        basicProperties: replyProps, 
                        body: responseBytes);

                    channel.BasicAck(
                        deliveryTag: ea.DeliveryTag, 
                        multiple: false);
                };

                Console.WriteLine(" Press [enter] to exit...");
                Console.ReadLine();
            }
        }

        private static int fib(int n)
        {
            if (n == 0 || n == 1)
            {
                return n;
            }
            return fib(n - 1) + fib(n - 2);
        }
    }
}
