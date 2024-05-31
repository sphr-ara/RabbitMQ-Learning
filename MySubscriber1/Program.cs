using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory
{
    HostName = "localhost"
};

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

// In case the consumer starts before producer, if it exists it won't do anything
channel.ExchangeDeclare(exchange: "pubsub", type: ExchangeType.Fanout);

var queueName = channel.QueueDeclare().QueueName;
// Get message just if your done with your previous task
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

var consumer = new EventingBasicConsumer(channel);

// channel.QueueBind(queue: queueName, exchange: "pubsub", routingKey: "");

consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Received message in 1: {message}");
};

channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

Console.ReadKey();