using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://vlukfgov:ssmrGwVrhO4lbGe2DZMPXJ8sjEfTLIzY@kebnekaise.lmq.cloudamqp.com/vlukfgov");

using IConnection connection = factory.CreateConnection();
using (IModel channel = connection.CreateModel())
{
    Byte[] message = Encoding.UTF8.GetBytes("Hello RabbitMQ");

    // QueueDeclare'deki parametrelerin aynı olduğundan emin ol.
    channel.QueueDeclare(queue: "hello-queue", durable: true, exclusive: false, autoDelete: false);

    channel.BasicPublish(exchange: "", routingKey: "hello-queue", body: message);
}

Console.WriteLine("Mesaj Gönderilmiştir.");
Console.ReadLine();
