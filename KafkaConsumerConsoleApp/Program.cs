using Confluent.Kafka;

var config = new ConsumerConfig
{
    BootstrapServers = "localhost:9092",
    GroupId = "test-group",
    AutoOffsetReset = AutoOffsetReset.Earliest

    //EnableAutoCommit = false,
    //StatisticsIntervalMs = 5000,
    //SessionTimeoutMs = 6000,
    //AutoOffsetReset = AutoOffsetReset.Earliest,
    //EnablePartitionEof = true,
    //// A good introduction to the CooperativeSticky assignor and incremental rebalancing:
    //// https://www.confluent.io/blog/cooperative-rebalancing-in-kafka-streams-consumer-ksqldb/
    //PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky
};

using var consumer = new ConsumerBuilder<Ignore, string>(config)
    .Build();

TopicPartitionOffset tps = new TopicPartitionOffset(
    new TopicPartition("test-topic", 0)
    , Offset.Beginning);

consumer.Assign(tps);

consumer.Subscribe("test-topic");
try
{
    while (true)
    {
        var message = consumer.Consume();
        Console.WriteLine($"Received message: {message.Message.Value}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}