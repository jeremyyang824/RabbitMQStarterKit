#!powershell
#Set-ExecutionPolicy UnRestricted

cd "D:\Program Files\RabbitMQ Server\rabbitmq_server-3.6.9"

#禁用web控制台插件
./sbin/rabbitmq-plugins disable rabbitmq_management

#启动3个RabbitMQ节点
"create rabbit server 1..."
$env:RABBITMQ_NODE_PORT=5672 
$env:RABBITMQ_NODENAME="rabbit"
ls env:RABBITMQ_NODE_PORT
ls env:RABBITMQ_NODENAME
./sbin/rabbitmq-server -detached
"create rabbit server 2..."
$env:RABBITMQ_NODE_PORT=5673 
$env:RABBITMQ_NODENAME="rabbit_1"
ls env:RABBITMQ_NODE_PORT
ls env:RABBITMQ_NODENAME
./sbin/rabbitmq-server -detached
"create rabbit server 3..."
$env:RABBITMQ_NODE_PORT=5674 
$env:RABBITMQ_NODENAME="rabbit_2"
ls env:RABBITMQ_NODE_PORT
ls env:RABBITMQ_NODENAME
./sbin/rabbitmq-server -detached

#将第2个节点加入群集，并设为磁盘节点
"join rabbit_1 to cluster..."
./sbin/rabbitmqctl -n rabbit_1 stop_app
./sbin/rabbitmqctl -n rabbit_1 reset
./sbin/rabbitmqctl -n rabbit_1 join_cluster rabbit@Jeremyyang-PC
./sbin/rabbitmqctl -n rabbit_1 start_app

#将第3个节点加入群集，并设为内存节点
"join rabbit_2 to cluster..."
./sbin/rabbitmqctl -n rabbit_2 stop_app
./sbin/rabbitmqctl -n rabbit_2 reset
./sbin/rabbitmqctl -n rabbit_2 join_cluster rabbit@Jeremyyang-PC --ram
./sbin/rabbitmqctl -n rabbit_2 start_app

#查看集群状态
./sbin/rabbitmqctl -n rabbit cluster_status

#启用web控制台插件(http://localhost:15672/mgmt)(guest:guest)
./sbin/rabbitmq-plugins enable rabbitmq_management
