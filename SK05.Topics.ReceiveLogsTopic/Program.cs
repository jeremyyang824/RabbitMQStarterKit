﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SK05.Topics.ReceiveLogsTopic
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
                    exchange: "topic_logs", 
                    type: "topic");

                var queueName = channel.QueueDeclare().QueueName;

                if (args.Length < 1)
                {
                    Console.Error.WriteLine("Usage: {0} [binding_key...]", Environment.GetCommandLineArgs()[0]);
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                    return;
                }

                foreach (var bindingKey in args)
                {
                    channel.QueueBind(
                        queue: queueName, 
                        exchange: "topic_logs", 
                        routingKey: bindingKey);
                }

                Console.WriteLine(" [*] Waiting for messages... To exit press CTRL+C");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;
                    Console.WriteLine(" SK05.Topics.ReceiveLogsTopic: '{0}':'{1}'", routingKey, message);
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
