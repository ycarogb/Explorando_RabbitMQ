﻿using System.Text;
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
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"[*] Received {message}");
};

//Consumir mensagem
channel.BasicConsume(
    queue: QueueName, 
    autoAck: true, 
    consumer: consumer);

Console.WriteLine("Press [enter] to exit.");
Console.ReadLine();
                    