using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

const string UriRabbitMQ = " ";
const string QueueName = "";
var factory = new ConnectionFactory() { Uri = new Uri(UriRabbitMQ) };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(
    queue: QueueName,
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null);

Console.WriteLine("[*] Waiting for messages.");

var consumer = new EventingBasicConsumer(channel);

//providencia um callback para aguardar a mensagem chegarem de forma assíncrona
//utilizando mais de um consumidor, o RabbitMQ permite paralelizar o trabalho, permitindo que as filas sejam consumidas por diferentes consumidores
    //a fila sempre será consumida pelo próximo consumidor, em sequência, a partir da quantidade de consumidores no modelo adotado
consumer.Received += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"[*] Received {message}");
    
    int dots = message.Split('.').Length -1;
    await Task.Delay(dots * 1000);
    
    Console.WriteLine("[*] Done");
    
    //Ack = Messages ACK(nowledgments) -> mensagem de confirmação enviada pelo consumidor de que a mensagem foi processada e pode ser deletada pelo RabbitMQ
        //Importante para garantir que nenhuma task será perdida se um consumidor "cair" por algum motivo
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
};

//Consumir mensagem
channel.BasicConsume(
    queue: QueueName, 
    autoAck: false, 
    consumer: consumer);

Console.WriteLine("Press [enter] to exit.");
Console.ReadLine();