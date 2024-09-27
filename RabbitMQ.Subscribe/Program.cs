using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://vlukfgov:ssmrGwVrhO4lbGe2DZMPXJ8sjEfTLIzY@kebnekaise.lmq.cloudamqp.com/vlukfgov");

IConnection connection = null;
IModel channel = null;

try
{
    connection = factory.CreateConnection();
    channel = connection.CreateModel();

    // Kuyruğu oluşturduğumuzdan emin olalım.
    channel.QueueDeclare(queue: "hello-queue", durable: true, exclusive: false, autoDelete: false);

    channel.BasicQos(0, 1, false);

    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

    channel.BasicConsume(queue: "hello-queue", autoAck: false, consumer: consumer);

    consumer.Received += (sender, e) =>
    {
        try
        {
            Thread.Sleep(1500);  // Simüle edilen işleme süresi.
            string message = Encoding.UTF8.GetString(e.Body.ToArray());
            Console.WriteLine($"{message} alındı");

            // Mesaj başarıyla işlendiğinde onayla.
            channel.BasicAck(e.DeliveryTag, false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Mesaj işlenirken bir hata oluştu: {ex.Message}");
            // Hata durumunda mesajı yeniden kuyruğa koyabilir ya da loglayabilirsiniz.
        }
    };


    // Kuyruktaki mevcut mesaj sayısını kontrol et.
    var queueInfo = channel.QueueDeclarePassive("hello-queue");
    Console.WriteLine($"Kuyrukta {queueInfo.MessageCount} mesaj var.");

    // Program sonlanmadan önce kanalı açık tutuyoruz.
    Console.WriteLine("Mesaj alımı için bekleniyor... Çıkmak için [Enter] tuşuna basın.");
    Console.ReadLine();
}
catch (Exception ex)
{
    Console.WriteLine($"Bir hata oluştu: {ex.Message}");
}
finally
{
    // Bağlantı ve kanalı dispose ederek temizleme işlemi.
    if (channel != null && channel.IsOpen)
        channel.Close();

    if (connection != null && connection.IsOpen)
        connection.Close();
}

Console.ReadLine();