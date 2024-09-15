using System.Text;

using Microsoft.Extensions.Configuration;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WareLogix.Messaging.RabbitMq;

public class RabbitMqConsumer : IMessageQueueConsumer<string>
{
    const string cloudAmqpUrlKey = "CloudAmqpUrl";

    private readonly Uri cloudAmqpUri;

    private readonly ConnectionFactory connectionFactory;
    

    //private Action<T> xx;
    //public delegate void Del<T>(T item);
    //public Action<T> m1;
    //private Action<object> xx;
    //private Action<T> messageHandler;

    public RabbitMqConsumer(IConfiguration configuration)
    {
        cloudAmqpUri = new Uri(configuration[cloudAmqpUrlKey] ?? throw new ConfigurationNullException(cloudAmqpUrlKey));

        connectionFactory = new ConnectionFactory
        {
            Uri = cloudAmqpUri
        };
    }
    
    public void ConsumeFromQueue(string queueName)
    {
        var connection = connectionFactory.CreateConnection();
        var channel = connection.CreateModel();

        EnsureQueueExists(channel, queueName);

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (model, deliveryEventArgs) =>
        {
            var body = deliveryEventArgs.Body.ToArray();
            // convert the message back from byte[] to a string
            var message = Encoding.UTF8.GetString(body);

            HandleMessage?.Invoke(message);

            //Console.WriteLine("** Received message: {0} by Consumer thread **", message);
            // ack the message, ie. confirm that we have processed it
            // otherwise it will be requeued a bit later
            channel.BasicAck(deliveryEventArgs.DeliveryTag, false);
        };

        _ = channel.BasicConsume(consumer, queueName);
        // Wait for the reset event and clean up when it triggers
        //_resetEvent.WaitOne();
        //channel?.Close();
        //channel = null;
        //connection?.Close();
        //connection = null;
    }

    private void EnsureQueueExists(IModel channel, string queueName)
    {
        bool durable = false;
        bool exclusive = false;
        bool autoDelete = true;

        channel.QueueDeclare(queueName, durable, exclusive, autoDelete, null);
    }

    private Action<string> HandleMessage;

    public void SetMessageHandler(Action<string> messageHandler)
    {
        HandleMessage = messageHandler;
    }
}
