﻿using System.Text;
using RabbitMQ.Client;

const string UriRabbitMQ = "";
const string QueueName = "";

var factory = new ConnectionFactory() { Uri = new Uri(UriRabbitMQ) };
using var connection = factory.CreateConnection(); //cria conexão com o servidor do RabbitMQ
using var channel = connection.CreateModel(); // onde reside a maior parte da API para realizar as tarefas; "pequenas" conexões que compartilham uma conexão TCP"

//Declarar uma fila 
channel.QueueDeclare(
    queue: QueueName, //nome
    durable: true, // fila deve ser recuperada com a perda de servidor? 
    exclusive: false, // fila deve ser usada somente na mesma conexão?
    autoDelete: false, // filla deve ser deletada quando o último consumidor for cancelado/finalizado
    arguments: null);

//Preparando a mensagem
var message = "Hello World!";
var body = Encoding.UTF8.GetBytes(message); //conteúdo da mensagem

//Publicar mensagem
channel.BasicPublish(
    exchange: string.Empty,
    routingKey: QueueName,
    basicProperties: null,
    body: body);

Console.WriteLine($"[x] Sent {message}");
Console.WriteLine("Press [Enter] to exit.");
Console.ReadLine();