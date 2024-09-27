using RabbitMQ.Client;
using System.Text;


ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://vlukfgov:ssmrGwVrhO4lbGe2DZMPXJ8sjEfTLIzY@kebnekaise.lmq.cloudamqp.com/vlukfgov");

using IConnection connection = factory.CreateConnection();
using (IModel channel = connection.CreateModel())
{
    channel.ExchangeDeclare("logs-fanout", type: ExchangeType.Fanout, durable: true, autoDelete: false);

    for (int i = 0; i < 50; i++)
    {
        Byte[] message = Encoding.UTF8.GetBytes($"Logs {i}");
        channel.BasicPublish(exchange: "logs-fanout", routingKey: "", body: message);
        Console.WriteLine($"Mesaj {i} Gönderilmiştir.");
    }
}

Console.ReadLine();


