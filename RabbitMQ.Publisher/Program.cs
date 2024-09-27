using RabbitMQ.Client;
using System.Text;

string[] LogNames = new string[] { "Critical", "Error", "Warning", "Info" };

ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://vlukfgov:ssmrGwVrhO4lbGe2DZMPXJ8sjEfTLIzY@kebnekaise.lmq.cloudamqp.com/vlukfgov");

using IConnection connection = factory.CreateConnection();

using (IModel channel = connection.CreateModel())
{
    channel.ExchangeDeclare("logs-direct", type: ExchangeType.Direct, durable: true, autoDelete: false);

    foreach (var item in LogNames)
    {
        var rootKey = $"root-{item}";

        var queueName = $"direct-queue-{item}";
        channel.QueueDeclare(queueName, true, false, false);
        channel.QueueBind(queueName, "logs-direct", rootKey, null);
    }

    for (int i = 0; i < 50; i++)
    {
        var logNamesItem = LogNames[new Random().Next(1, 5)];
        var rootKey = $"root-{logNamesItem}";

        Byte[] message = Encoding.UTF8.GetBytes($"Log {logNamesItem} - {i}");
        channel.BasicPublish(exchange: "logs-direct", routingKey: rootKey, body: message);
        Console.WriteLine($"Log {i} Gönderilmiştir.");
    }
}



Console.ReadLine();


