using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

#region
//HATA

//Kodunuzdaki potansiyel hataların birkaç nedeni olabilir. Hatanın detaylarına geçmeden önce, 
//ana konulara odaklanalım:

//Bağlantı ve Kanalın Erken Kapanması: Kodun sonunda finally bloğunda bağlantı ve kanal 
//    kapatılıyor. Ancak, Tüketici (consumer) hala çalışmaya devam etmeye çalışırken kanal 
//    kapanmış olabilir. Bu nedenle, channel.Close() ve connection.Close() işlemleri, 
//    Console.ReadLine()'dan sonra yapılmalı.

//Çözüm: finally bloğunda bağlantı ve kanal kapatılmadan önce programın mesaj alımını 
//    tamamlaması için bekleyin.

//QueueDeclare ile Geçici Kuyruk Kullanımı: channel.QueueDeclare() ile bir geçici kuyruk 
//    oluşturuluyor (var queueRandomName = channel.QueueDeclare().QueueName;), ancak bu kuyruk, 
//    bağlantı sona erdiğinde otomatik olarak silinir. Bu geçici kuyruklar, 
//    bağlantı sona erdiğinde kendiliğinden yok olduğu için, bu durumu bilerek hareket 
//    etmelisiniz. Eğer uzun süreli bir kuyruk istiyorsanız, durable özelliğini true 
//    yaparak kalıcı bir kuyruk oluşturmanız gerekir.

//Olay Kaydından Önce BasicConsume Çağrısı: BasicConsume çağrısını yaparken, tüketici 
//    (consumer) için Received olayına bağlı işlemleri kaydetmeden önce yapıyorsunuz. 
//    Bu sıralama bir sorun oluşturmasa da, genellikle önce Received olayını tanımlayıp 
//    sonra BasicConsume çağrısı yapmak tercih edilir. Bu, olayların tüketiciyi beklemesi
//    için daha güvenilir bir yöntemdir.

//Düzenlemeler:

//BasicConsume ve Received Olayı: consumer.Received olayını önce tanımladık,
//sonra BasicConsume'u çağırdık. Bu şekilde, olası bir senkronizasyon sorunundan kaçınıyoruz.

//finally Bloğunun Düzenlenmesi: Bağlantı ve kanal kapanışını Console.ReadLine()'dan sonra
//bıraktık. Bu şekilde program, tüm mesajları işleyene kadar açık kalacak.

#endregion

ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://vlukfgov:ssmrGwVrhO4lbGe2DZMPXJ8sjEfTLIzY@kebnekaise.lmq.cloudamqp.com/vlukfgov");

IConnection connection = null;
IModel channel = null;

try
{
    connection = factory.CreateConnection();
    channel = connection.CreateModel();

    // Fanout tipinde bir exchange oluşturuyoruz
    channel.ExchangeDeclare("logs-fanout", type: ExchangeType.Fanout, durable: true, autoDelete: false);

    // Geçici kuyruk oluştur
    var queueRandomName = channel.QueueDeclare().QueueName;

    // Kuyruğu exchange'e bağla
    channel.QueueBind(queueRandomName, "logs-fanout", "", null);

    // Basic QoS (Quality of Service) ile mesajların birer birer işlenmesini sağla
    channel.BasicQos(0, 1, false);

    // Consumer oluştur
    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

    // Tüketici olayını tanımla
    consumer.Received += (sender, e) =>
    {
        try
        {
            Thread.Sleep(1500);  // Simüle edilen işleme süresi
            string message = Encoding.UTF8.GetString(e.Body.ToArray());
            Console.WriteLine($"{message} alındı");

            // Mesaj başarıyla işlendiğinde onayla (ACK)
            channel.BasicAck(e.DeliveryTag, false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Mesaj işlenirken bir hata oluştu: {ex.Message}");
            // Hata durumunda mesajı yeniden kuyruğa koyabilir ya da loglayabilirsiniz
        }
    };

    // Mesajları tüket
    channel.BasicConsume(queue: queueRandomName, autoAck: false, consumer: consumer);

    Console.WriteLine("Loglar yazılıyor... Çıkmak için [Enter] tuşuna basın.");
    Console.ReadLine();
}
catch (Exception ex)
{
    Console.WriteLine($"Bir hata oluştu: {ex.Message}");
}
finally
{
    // Bağlantı ve kanalın kapanma işlemi
    if (channel != null && channel.IsOpen)
        channel.Close();

    if (connection != null && connection.IsOpen)
        connection.Close();
}
