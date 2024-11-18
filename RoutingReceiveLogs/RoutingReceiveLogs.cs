using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitmqConfigs;

if (args.Length < 1)
{
    Console.Error.WriteLine("Usage: {0} [info] [warning] [error]",
        Environment.GetCommandLineArgs()[0]);
    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
    Environment.ExitCode = 1;
    return;
}

var factory = new ConnectionFactory() { Uri = new Uri(RabbitmqConnection.UriConnection) };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(
    exchange: "direct_logs",
    type: ExchangeType.Direct);

var queueDeclareResult = channel.QueueDeclare(); //usar o QueueDeclare SEM PARAMETROS cria uma fila não durável, exclusiva, que pode apagar a si mesma com um nome gerado pelo RabbitMQ
var queueName = queueDeclareResult.QueueName;

foreach (var severity in args)
{
    channel.QueueBind(
        queue: queueName,
        exchange: "direct_logs",
        routingKey: severity); //binding = estrutura que relaciona exchange e filas
}

Console.WriteLine(" [*] Waiting for logs...");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    byte[] body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var routingKey = ea.RoutingKey;
    Console.WriteLine($" [x] Received '{routingKey}' : '{message}'");
};

channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

Console.WriteLine(" [*] Press [enter] to exit.");
Console.ReadLine();