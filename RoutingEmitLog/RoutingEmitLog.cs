using System.Text;
using RabbitMQ.Client;
using RabbitmqConfigs;

var factory = new ConnectionFactory() { Uri = new Uri(RabbitmqConnection.UriConnection) };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

//Declara exchange - Recebe a mensagem de um produtor e distribui para as filas
channel.ExchangeDeclare(
    exchange: "direct_logs", 
    type: ExchangeType.Direct);

//Preparando a mensagem
var message = GetMessage(args);
var body = Encoding.UTF8.GetBytes(message);
var severity = args.Length > 0 ? args[0] : "info";

channel.BasicPublish(
    exchange: "direct_logs", 
    routingKey: severity,
    body: body);

Console.WriteLine(" [x] Sent {0}", message);
Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

static string GetMessage(string[] args)
{
    return ((args.Length > 0) ? string.Join(" ", args) : "info: Hello World!");
}