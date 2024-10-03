using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

// http://172.20.39.125:30228

//ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("172.20.39.125:30228");
//ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("redis");
ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:26379");
IDatabase db = redis.GetDatabase();


db.StringSet("foo", "bar");
Console.WriteLine(db.StringGet("foo")); // prints bar

//Console.WriteLine(db.StringGet("gaga")); // prints bar

