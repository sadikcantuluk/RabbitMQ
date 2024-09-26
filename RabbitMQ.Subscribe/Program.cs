using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://vlukfgov:ssmrGwVrhO4lbGe2DZMPXJ8sjEfTLIzY@kebnekaise.lmq.cloudamqp.com/vlukfgov");

using IConnection connection = factory.CreateConnection();
using (IModel channel = connection.CreateModel())
{
    // Aynı QueueDeclare parametrelerini kullanıyoruz.
    channel.QueueDeclare(queue: "hello-queue", durable: true, exclusive: false, autoDelete: false);

    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

    // AutoAck: true -> Mesaj alındıktan hemen sonra otomatik olarak kuyruktan silinsin.
    channel.BasicConsume(queue: "hello-queue", autoAck: true, consumer: consumer);

    consumer.Received += (sender, e) =>
    {
        Console.WriteLine("Mesaj alındı!");
        var mesaj = Encoding.UTF8.GetString(e.Body.ToArray());
        Console.WriteLine($"Mesaj: {mesaj}");
    };

    // Kuyruktaki mesaj sayısını kontrol edelim.
    var queueInfo = channel.QueueDeclarePassive("hello-queue");
    Console.WriteLine($"Kuyrukta {queueInfo.MessageCount} mesaj var.");
}

Console.ReadLine();
