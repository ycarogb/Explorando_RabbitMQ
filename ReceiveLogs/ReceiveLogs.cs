using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitmqConfigs;

var factory = new ConnectionFactory() { Uri = new Uri(RabbitmqConnection.UriConnection) };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(
    exchange: "logs",
    type: ExchangeType.Fanout);

var queueDeclareResult = channel.QueueDeclare(); //usar o QueueDeclare SEM PARAMETROS cria uma fila não durável, exclusiva, que pode apagar a si mesma com um nome gerado pelo RabbitMQ
var queueName = queueDeclareResult.QueueName;
channel.QueueBind(
    queue: queueName,
    exchange: "logs",
    routingKey: string.Empty); //binding = estrutura que relaciona exchange e filas

Console.WriteLine(" [*] Waiting for logs...");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    byte[] body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine(" [x] {0}", message);
};

channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

Console.WriteLine(" [*] Press [enter] to exit.");
Console.ReadLine();