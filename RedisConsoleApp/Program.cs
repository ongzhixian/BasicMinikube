using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

// http://172.20.39.125:30228

//ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("172.20.39.125:30228");
//ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("redis");
ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:26379");
IDatabase db = redis.GetDatabase();

db.StringSet("foo", "bar7");

for (int i = 0; i < 9999; i++) Console.WriteLine(db.StringGet("foo")); // prints bar
