using System;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using TradingBot.Models;

using WareLogix;

namespace TradingConsoleApp.Services;

public class OandaStreamingService
{

    private readonly ILogger<OandaStreamingService> logger;
    private readonly HttpClient httpClient;

    public OandaStreamingService(ILogger<OandaStreamingService> logger, IConfiguration configuration, HttpClient httpClient)
    {
        this.logger = logger;
        this.httpClient = httpClient;
        this.httpClient.BaseAddress = new Uri(configuration["Oanda:StreamingApiUrl"] ?? throw new ConfigurationNullException("Oanda:StreamingApiUrl"));

        // Configure HttpClient headers
        SetAcceptHeader();
        SetAuthorizationHeader(configuration);
    }

    public async Task GetPriceStreamAsync(string? accountId)
    {
        if (accountId == null) return;

        string apiUrl = $"/v3/accounts/{accountId}/pricing/stream?instruments=XAU_USD";

        // As of .NET 7, we can annotate data types with `JsonPolymorphic` and `JsonDerivedType` attributes
        //JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        //options.Converters.Add(new PriceStreamDataJsonConverter());

        using var stream = await httpClient.GetStreamAsync(apiUrl);
        using var streamReader = new StreamReader(stream);
        while (!streamReader.EndOfStream)
        {
            var content = await streamReader.ReadLineAsync();

            if (content == null) continue;

            var result = JsonSerializer.Deserialize<PriceStreamData>(content);

            if (result is ClientPrice priceInfo)
            {
                Console.WriteLine($"Bid: {priceInfo.bids.FirstOrDefault().price} {priceInfo.bids.LastOrDefault().price} \tAsk: {priceInfo.asks.FirstOrDefault().price} {priceInfo.asks.LastOrDefault().price} ");
            }
            
            


            //Console.WriteLine($"Read {content}");
        }

        // ZX: Why we cannot use DeserializeAsyncEnumerable;
        // From the docs:
        // The response body for the Pricing Stream uses chunked transfer encoding.
        // Each chunk contains Price and/or PricingHeartbeat objects encoded as JSON.
        // Each JSON object is serialized into a single line of text, and multiple objects found in the same chunk are separated by newlines.
        // Which means the JSON data stream has new line characters in it (ie. its not a pure JSON stream)
        //await foreach (var item in JsonSerializer.DeserializeAsyncEnumerable<PricingHeartbeat>(stream, options))
        //{
        //    //Console.WriteLine($"Id: {item.Id}, Name: {item.Name}");
        //    Console.WriteLine($"Read 1");
        //}
    }


    // PRIVATE METHODS

    // SET HTTP HEADERS

    private void SetAuthorizationHeader(IConfiguration configuration)
    {
        //this.httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuration["OandaAccessToken"]}");
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", configuration["OandaAccessToken"]);
    }

    private void SetAcceptHeader()
    {
        //this.httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        //httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(System.Net.Mime.MediaTypeNames.Application.Octet));
    }

}
