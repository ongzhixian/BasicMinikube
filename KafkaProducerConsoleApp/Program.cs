using Confluent.Kafka;

var config = new ProducerConfig
{
    BootstrapServers = "localhost:9092",
    //LingerMs = 0,
    //Acks = 0,
};


using (var adminClient = new AdminClientBuilder(new AdminClientConfig
{
    BootstrapServers = "localhost:9092",

}).Build())
{
    
    var meta = adminClient.GetMetadata(TimeSpan.FromSeconds(20));

    meta.Topics.ForEach(topic =>
    {
        Console.WriteLine($"Topic: {topic.Topic} ; Status: {topic.Error} ; Partition count: {topic.Partitions.Count}");
    });
}




Console.WriteLine("All done");
return;

var topic = "test-topic";

using var producer = new ProducerBuilder<Null, string>(config).Build();

//for (int i = 0; i < args.Length; i++)
for (int i = 0; i < 999; i++)
{
    var message = new Message<Null, string>
    {
        //Value = args[i]
        Value = $"Test message {i} sent at {DateTime.Now:O}"
    };

    producer.Produce(
        topic
        , message
        , deliveryReport => {
            
            Console.WriteLine($"{deliveryReport.Error} -- {deliveryReport.Message.Value}");
        });

    producer.Flush();

    Console.WriteLine($"Message {i} sent");

    Thread.Sleep(5000);
}

