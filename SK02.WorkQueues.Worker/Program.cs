using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SK02.WorkQueues.Worker
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

                channel.BasicQos(
                    prefetchSize: 0,
                    prefetchCount: 1,   //确保每个Consumer同一时间最多处理一个消息(Consumer在ack前，将不会分发新的消息给它)
                    global: false);     //false: Qos应用于per-consumer; true: Qos应用于per-channel

                Console.WriteLine(" [*] Waiting for messages...");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" SK02.WorkQueues.Worker:{0}", message);

                    int dots = message.Split('.').Length - 1;
                    Thread.Sleep(dots * 1000);
                    Console.WriteLine(" [x] Done");

                    //确认"收到"消息
                    channel.BasicAck(
                        deliveryTag: ea.DeliveryTag,    //Confirm消息的序列号
                        multiple: false);               //true: Ack所有拥有相同DeliveryTag的消息

                    //channel.BasicNack
                };

                channel.BasicConsume(
                    queue: "task_queue",
                    noAck: false,
                    consumer: consumer);

                Console.WriteLine(" Press [enter] to exit...");
                Console.ReadLine();
            }
        }
    }
}
