using StackExchange.Redis;

string redisConnectionString = "localhost:26379";
string redisTestListName = "TestList";

CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
CancellationToken cancellationToken = cancellationTokenSource.Token;


ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConnectionString);
ISubscriber subscriber = redis.GetSubscriber();
RedisChannel testListChannel = new("TestListChannel", RedisChannel.PatternMode.Literal);

ChannelMessageQueue channelMessageQueue = await subscriber.SubscribeAsync(testListChannel);
channelMessageQueue.OnMessage(ChannelMessageHandler);

Console.CancelKeyPress += Console_CancelKeyPress;

void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
{
    cancellationTokenSource.Cancel();
    e.Cancel = true;
    
}

void ChannelMessageHandler(ChannelMessage channelMessage)
{
    Console.WriteLine(channelMessage.Message);
}


Console.WriteLine("[PROGRAM START]");

try
{
    while (!cancellationToken.IsCancellationRequested)
    {
        TimeSpan pingLatency = subscriber.Ping();

        Console.WriteLine($"Ping latency is {pingLatency}");

        await Task.Delay(1000 * 3, cancellationToken);
    }
}
catch (TaskCanceledException)
{
    Console.WriteLine("AAA");
    //throw;
}

Console.WriteLine("[PROGRAM END]");