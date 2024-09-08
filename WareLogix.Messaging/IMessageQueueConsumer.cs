namespace WareLogix.Messaging;

public interface IMessageQueueConsumer
{
    //void Publish(string message);
    //void PublishToQueue(string queueName, string message);
    void ConsumeFromQueue(string queueName);

}
