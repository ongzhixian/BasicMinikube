namespace WareLogix.Messaging;

public interface IMessageQueueConsumer<T>
{
    //void Publish(string message);
    //void PublishToQueue(string queueName, string message);
    void ConsumeFromQueue(string queueName);

    void SetMessageHandler(Action<T> messageHandler);
}
