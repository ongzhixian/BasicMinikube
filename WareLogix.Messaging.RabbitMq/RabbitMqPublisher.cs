using System.Text;

using Microsoft.Extensions.Configuration;

using RabbitMQ.Client;

namespace WareLogix.Messaging.RabbitMq;

public class RabbitMqPublisher : IMessageQueuePublisher
{
    const string cloudAmqpUrlKey = "CloudAmqpUrl";

    private readonly Uri cloudAmqpUri;

    private readonly ConnectionFactory connectionFactory;

    public RabbitMqPublisher(IConfiguration configuration)
    {
        cloudAmqpUri = new Uri(configuration[cloudAmqpUrlKey] ?? throw new ConfigurationNullException(cloudAmqpUrlKey));

        connectionFactory = new ConnectionFactory
        {
            Uri = cloudAmqpUri
        };
    }

    public void PublishToQueue(string queueName, string message)
    {
        using var connection = connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();

        EnsureQueueExists(channel, queueName);

        var data = Encoding.UTF8.GetBytes(message);
        var exchangeName = string.Empty;
        var routingKey = queueName;
        channel.BasicPublish(exchangeName, routingKey, null, data);
    }

    private void EnsureQueueExists(IModel channel, string queueName)
    {
        bool durable = false;
        bool exclusive = false;
        bool autoDelete = true;

        channel.QueueDeclare(queueName, durable, exclusive, autoDelete, null);
    }
}
