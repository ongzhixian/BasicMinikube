namespace TradingConsoleApp.Services;

using System.Diagnostics;
using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using RabbitMQ.Client;

using Serilog.Core;

using WareLogix.Messaging;
using WareLogix.Messaging.RabbitMq;

internal class CloudAmqpDemoService : BackgroundService
{
    private readonly ILogger<CloudAmqpDemoService> logger;
    
    private readonly IMessageQueuePublisher messageQueuePublisher;

    public CloudAmqpDemoService(ILogger<CloudAmqpDemoService> logger, IMessageQueuePublisher messageQueuePublisher)
    {
        this.logger = logger;
        this.messageQueuePublisher = messageQueuePublisher;
    }

    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        int i = 0;

        while (!stoppingToken.IsCancellationRequested)
        {
            var message = $"Message {i++} - {DateTime.Now:O}";

            logger.LogInformation("Send [{Message}]", message);

            messageQueuePublisher.PublishToQueue("queue1", message);

            await Task.Delay(3600, stoppingToken);
        }
            
            
        
        //return Task.CompletedTask;
        //// TODO: Dump this to RabbitMq

        //// Create a ConnectionFactory and set the Uri to the CloudAMQP url
        //// the connectionfactory is stateless and can safetly be a static resource in your app
        //var factory = new ConnectionFactory
        //{
        //    Uri = cloudAmqpUri
        //};

        //// create a connection and open a channel, dispose them when done
        //using var connection = factory.CreateConnection();
        //using var channel = connection.CreateModel();

        //// ensure that the queue exists before we publish to it
        //var queueName = "queue1";
        //bool durable = false;
        //bool exclusive = false;
        //bool autoDelete = true;

        //channel.QueueDeclare(queueName, durable, exclusive, autoDelete, null);

        //// read message from input
        //var message = "Hello";

        //// the data put on the queue must be a byte array
        //var data = Encoding.UTF8.GetBytes(message);

        //// publish to the "default exchange", with the queue name as the routing key
        //var exchangeName = string.Empty;
        //var routingKey = queueName;
        //channel.BasicPublish(exchangeName, routingKey, null, data);

    }


    protected async Task xxxExecuteAsyncs(CancellationToken stoppingToken)
    {
        int i = 0;
        Uri slowUrl = new Uri("https://localhost:5207/200?sleep=1000");
        Uri fastUrl = new Uri("https://localhost:5207/200");

        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine($"bean {i++}");

            ActivitySource MyActivitySource = new("OpenTelemetry.Demo.SimpleApi");

            using var parent = MyActivitySource.StartActivity("SimpleApi");

            using (var client = new HttpClient())
            {
                using (var slow = MyActivitySource.StartActivity("SomethingSlow"))
                {
                    await client.GetStringAsync(slowUrl);
                    await client.GetStringAsync(slowUrl);
                }

                using (var fast = MyActivitySource.StartActivity("SomethingFast"))
                {
                    await client.GetStringAsync(fastUrl);
                }
            }

            this.logger.LogInformation("SimpleApiDemoService running at: {time}", DateTimeOffset.Now);
            await Task.Delay(3600, stoppingToken);
            
            //Thread.Sleep(3600);
            //await Task.Delay(1000, stoppingToken);
        }
        //Once execution leaves this method, it will not be called again.
    }
}
