namespace WareLogix.Messaging;

public interface IMessageQueuePublisher
{
    //void Publish(string message);
    void PublishToQueue(string queueName, string message);
}
