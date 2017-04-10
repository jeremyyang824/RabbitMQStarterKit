using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace SK01.HelloWorld.Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            IConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };

            //the connection abstracts the socket connection
            using (IConnection connection = factory.CreateConnection())
            //channel, session, and model
            using (IModel channel = connection.CreateModel())
            {
                //队列声明操作是幂等的
                channel.QueueDeclare(
                    queue: "hello_queue",
                    durable: false,     //是否持久化队列（非持久化消息）
                    exclusive: false,   //是否专有（匿名）队列
                    autoDelete: false,  //是否在最后一个consumer取消订阅后自动删除队列
                    arguments: null);

                string message = "Hello World!";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(
                    exchange: "",           //默认交换机（direct类型）
                    routingKey: "hello_queue",    //路由键（路由到对应名称的queue）
                    basicProperties: null,  
                    body: body);

                Console.WriteLine(" SKO1.HelloWorld.Sender: {0}", message);
            }

            Console.WriteLine(" Press [Enter] to exit...");
            Console.ReadLine();
        }
    }
}
