using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SK06.RPC.Client
{
    public class RpcClient : IDisposable
    {
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string replyQueueName;
        private readonly EventingBasicConsumer consumer;

        public RpcClient()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            //匿名队列作为RPC响应接收队列
            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(
                queue: replyQueueName,
                noAck: true,
                consumer: consumer);
        }

        public void CallAsync(string message, Action<string> callback)
        {
            var corrId = Guid.NewGuid().ToString();
            var props = channel.CreateBasicProperties();
            props.ReplyTo = replyQueueName;     //以响应消息的接收（匿名）队列名作为ReplyTo属性随请求发送
            props.CorrelationId = corrId;       //创建唯一的CorrelationId属性随请求发送

            //组织RPC请求消息
            var messageBytes = Encoding.UTF8.GetBytes(message);

            //RPC响应处理
            consumer.Received += (model, ea) =>
            {
                //验证响应消息的CorrelationId是否与请求消息的CorrelationId匹配
                if (ea.BasicProperties.CorrelationId != corrId)
                    return;

                var resp = Encoding.UTF8.GetString(ea.Body);

                //执行结果回调
                callback?.Invoke(resp);
            };

            //RPC请求消息发送至请求队列（rpc_queue）
            channel.BasicPublish(
                exchange: "",
                routingKey: "rpc_queue",
                basicProperties: props,
                body: messageBytes);
        }

        public void Close()
        {
            channel.Close();
            connection.Close();
        }

        public void Dispose()
        {
            this.Close();
        }
    }
}
