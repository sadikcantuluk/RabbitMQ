using RabbitMQ.Client;
using System.Text;

string[] LogNames = new string[] { "Critical", "Error", "Warning", "Info" };

ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://vlukfgov:ssmrGwVrhO4lbGe2DZMPXJ8sjEfTLIzY@kebnekaise.lmq.cloudamqp.com/vlukfgov");

using IConnection connection = factory.CreateConnection();

using (IModel channel = connection.CreateModel())
{
    channel.ExchangeDeclare("logs-topic", type: ExchangeType.Topic, durable: true, autoDelete: false);

    for (int i = 0; i < 50; i++)
    {
        var rnd = new Random();

        var log1 = LogNames[rnd.Next(0, 4)];
        var log2 = LogNames[rnd.Next(0, 4)];
        var log3 = LogNames[rnd.Next(0, 4)];

        var rootKey = $"{log1}.{log2}.{log3}";

        Byte[] message = Encoding.UTF8.GetBytes($"Log - {rootKey} - {i}. Mesaj");
        channel.BasicPublish(exchange: "logs-topic", routingKey: rootKey, body: message);
        Console.WriteLine($"Log {i} Gönderilmiştir.");
    }
}

Console.ReadLine();


