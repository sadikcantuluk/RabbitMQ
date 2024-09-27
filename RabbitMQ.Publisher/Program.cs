using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://vlukfgov:ssmrGwVrhO4lbGe2DZMPXJ8sjEfTLIzY@kebnekaise.lmq.cloudamqp.com/vlukfgov");

using IConnection connection = factory.CreateConnection();
using (IModel channel = connection.CreateModel())
{
    // QueueDeclare'deki parametrelerin aynı olduğundan emin ol.
    channel.QueueDeclare(queue: "hello-queue", durable: true, exclusive: false, autoDelete: false);

    for (int i = 0; i < 50; i++)
    {
        Byte[] message = Encoding.UTF8.GetBytes($"Mesaj {i}");
        channel.BasicPublish(exchange: "", routingKey: "hello-queue", body: message);
        Console.WriteLine($"Mesaj {i} Gönderilmiştir.");
    }
}

Console.ReadLine();
