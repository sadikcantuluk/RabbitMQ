using RabbitMQ.Client;
using System.Text;

string[] LogNames = new string[] { "Critical", "Error", "Warning", "Info" };

ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://vlukfgov:ssmrGwVrhO4lbGe2DZMPXJ8sjEfTLIzY@kebnekaise.lmq.cloudamqp.com/vlukfgov");

using IConnection connection = factory.CreateConnection();

using (IModel channel = connection.CreateModel())
{
    channel.ExchangeDeclare("header-exchange", type: ExchangeType.Headers, durable: true, autoDelete: false);

    Dictionary<string, object> headers = new Dictionary<string, object>();

    headers.Add("format", "PDF");
    headers.Add("shape", "A4");

    var properties = channel.CreateBasicProperties();
    properties.Headers = headers;

    channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes("Hedare Exchange mesajı."));

    Console.WriteLine("Mesaj gönderilmiştir.");
}

Console.ReadLine();


