using System.Text.Json;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using TechnicalAnalysisConsoleApp.Models;

using WareLogix.Messaging;

namespace TechnicalAnalysisConsoleApp.Services;

internal class CloudAmqpDemoService : BackgroundService
{
    private readonly ILogger<CloudAmqpDemoService> logger;

    private readonly IMessageQueueConsumer<string> messageQueueConsumer;

    private readonly TradableInstrumentService tradableInstrumentService;

    private Dictionary<string, long>? instrumentTypes;

    public CloudAmqpDemoService(ILogger<CloudAmqpDemoService> logger, 
        IMessageQueueConsumer<string> messageQueueConsumer,
        TradableInstrumentService tradableInstrumentService)
    {
        this.logger = logger;
        this.messageQueueConsumer = messageQueueConsumer;
        this.tradableInstrumentService = tradableInstrumentService;
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
            messageQueueConsumer.SetMessageHandler(TradableInstrumentsMessageHandler);
            messageQueueConsumer.ConsumeFromQueue("tradable-instruments");
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

    private void TradableInstrumentsMessageHandler(string message)
    {
        try
        {
            if (instrumentTypes == null)
                instrumentTypes = tradableInstrumentService.GetOandaIntrumentTypes();

            JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            var instrument = System.Text.Json.JsonSerializer.Deserialize<Instrument>(message, options);

            var instrument_id = tradableInstrumentService.AddOandaInstrument(
                instrument.name,
                instrumentTypes[instrument.type],
                instrument.displayName
                );

            //
            //instrument.tags
            if (instrument_id != 0)
            {
                foreach (var tag in instrument.tags)
                {
                    tradableInstrumentService.AddOandaInstrumentTag(instrument_id,
                        tag.type,
                        tag.name);
                }
            }

            Console.WriteLine("** Received message: {0} by Consumer thread **", message);
        }
        catch (Exception)
        {
            throw;
        }
        
    }

}