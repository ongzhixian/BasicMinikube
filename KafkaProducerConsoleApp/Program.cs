using Confluent.Kafka;

var config = new ProducerConfig
{
    BootstrapServers = "localhost:9092",
    //LingerMs = 0,
    //Acks = 0,
};

var topic = "test-topic";

using var producer = new ProducerBuilder<Null, string>(config).Build();

for (int i = 0; i < args.Length; i++)
{
    var message = new Message<Null, string>
    {
        Value = args[i]
    };

    producer.Produce(
        topic
        , message
        , deliveryReport => {
            Console.WriteLine(deliveryReport.Message.Value);
        });

    producer.Flush();
}

