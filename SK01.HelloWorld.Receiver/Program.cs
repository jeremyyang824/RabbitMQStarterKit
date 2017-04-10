using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SK01.HelloWorld.Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            IConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };
            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                //We might start the consumer before the publisher
                channel.QueueDeclare(
                    queue: "hello_queue",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                //消费者定义
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" SKO1.HelloWorld.Receiver: {0}", message);
                };

                //消费者绑定到队列
                channel.BasicConsume(
                    queue: "hello_queue",
                    noAck: true,            //是否不需要消费者手工确认消息
                    consumer: consumer);

                Console.WriteLine(" Press [Enter] to exit...");
                Console.ReadLine();
            }
        }
    }
}
