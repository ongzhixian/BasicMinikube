using StackExchange.Redis;

string redisConnectionString = "localhost:26379";
string redisTestListName = "TestList";

ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConnectionString);
IDatabase db = redis.GetDatabase();
ISubscriber subscriber = redis.GetSubscriber();
RedisChannel testListChannel = new("TestListChannel", RedisChannel.PatternMode.Literal);

for (int i = 0; i < 9999; i++)
{
    //long listLength = await db.ListRightPushAsync(redisTestListName, new RedisValue("Test message"));
    
    //Console.WriteLine($"New list length is {listLength}");

    long receipients = await subscriber.PublishAsync(testListChannel, new RedisValue($"My message number {i}"));

    Console.WriteLine($"Message sent to {receipients} receipient(s)");

    await Task.Delay(1000 * 10);
}
