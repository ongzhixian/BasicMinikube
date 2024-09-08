using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;

using WareLogix.Messaging;

namespace TechnicalAnalysisConsoleApp.Services;

internal class CloudAmqpDemoService : BackgroundService
{
    private readonly ILogger<CloudAmqpDemoService> logger;

    private readonly IMessageQueueConsumer messageQueueConsumer;

    public CloudAmqpDemoService(ILogger<CloudAmqpDemoService> logger, IMessageQueueConsumer messageQueueConsumer)
    {
        this.logger = logger;
        this.messageQueueConsumer = messageQueueConsumer;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {

        //int i = 0;
        //while (!stoppingToken.IsCancellationRequested)
        //{
        //    messageQueueConsumer.ConsumeFromQueue("queue1");
        //    //messageQueuePublisher.PublishToQueue("queue1", $"Message {i} - {DateTime.Now:O}");

        //    //await Task.Delay(3600, stoppingToken);
        //}

        return Task.Run(() =>
        {
            messageQueueConsumer.ConsumeFromQueue("queue1");
        });


        //// add the message receive event
        //consumer.Received += (model, deliveryEventArgs) =>
        //{
        //    var body = deliveryEventArgs.Body.ToArray();
        //    // convert the message back from byte[] to a string
        //    var message = Encoding.UTF8.GetString(body);
        //    Console.WriteLine("** Received message: {0} by Consumer thread **", message);
        //    // ack the message, ie. confirm that we have processed it
        //    // otherwise it will be requeued a bit later
        //    _channel.BasicAck(deliveryEventArgs.DeliveryTag, false);
        //};

        //// start consuming
        //_ = _channel.BasicConsume(consumer, queueName);
        //// Wait for the reset event and clean up when it triggers
        ////_resetEvent.WaitOne();
        //_channel?.Close();
        //_channel = null;
        //_connection?.Close();
        //_connection = null;
    }
}