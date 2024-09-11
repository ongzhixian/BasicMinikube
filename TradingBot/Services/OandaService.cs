using System.Net.Http.Json;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using WareLogix;

namespace TradingConsoleApp.Services;

public class OandaService
{

    private readonly ILogger<OandaService> logger;
    private readonly HttpClient httpClient;

    public OandaService(ILogger<OandaService> logger, IConfiguration configuration, HttpClient httpClient)
    {
        this.logger = logger;
        this.httpClient = httpClient;
        this.httpClient.BaseAddress = new Uri(configuration["Oanda:RestApiUrl"] ?? throw new ConfigurationNullException("Oanda:RestApiUrl"));
        
        // Configure HttpClient headers
        SetAcceptHeader();
        SetAuthorizationHeader(configuration);
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
        httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json));
    }

    
}

