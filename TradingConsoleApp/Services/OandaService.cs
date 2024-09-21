using System.Collections.Specialized;
using System.Net.Http.Json;
using System.Web;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using TradingConsoleApp.Models.OandaApi;

using WareLogix;
using WareLogix.Models.OandaApiModels;

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

    // ORDER ENDPOINTS

    public async Task CreateOrderAsync(string? accountId, LimitOrder order)
    {
        if (accountId == null) return;

        string apiUrl = $"/v3/accounts/{accountId}/orders";

        OrderRequest orderRequest = new OrderRequest
        {
            Order = order
        };

        System.Text.Json.JsonSerializerOptions options = new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web);
        options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;

        var x = System.Text.Json.JsonSerializer.Serialize(orderRequest, options);

        var responseMessage = await httpClient.PostAsJsonAsync(apiUrl, orderRequest);
        File.WriteAllText($"create-order.json", await responseMessage.Content.ReadAsStringAsync());
        logger.LogInformation($"Response status code: {responseMessage.StatusCode}");

        //var response = await httpClient.GetFromJsonAsync<GetInstrumentPositionBookResponse>(apiUrl);
        //logger.LogInformation($"Response: {response}");
        //System.Diagnostics.Debugger.Break();
    }

    public async Task CancelPendingOrderAsync(string? accountId, string orderId)
    {
        if (accountId == null) return;

        string apiUrl = $"/v3/accounts/{accountId}/orders/{orderId}/cancel";

        StringContent emptyStringContent = new StringContent(string.Empty);
        var responseMessage = await httpClient.PutAsync(apiUrl, emptyStringContent);
        File.WriteAllText($"{orderId}-cancel-pending-order.json", await responseMessage.Content.ReadAsStringAsync());
        logger.LogInformation($"Response status code: {responseMessage.StatusCode}");

        //var response = await httpClient.GetFromJsonAsync<GetInstrumentPositionBookResponse>(apiUrl);
        //logger.LogInformation($"Response: {response}");
        //System.Diagnostics.Debugger.Break();
    }


    public async Task GetOrdersAsync(string? accountId)
    {
        if (accountId == null) return;

        string apiUrl = $"/v3/accounts/{accountId}/orders";

        var responseMessage = await httpClient.GetAsync(apiUrl);
        File.WriteAllText($"{accountId}-orders.json", await responseMessage.Content.ReadAsStringAsync());
        logger.LogInformation($"Response status code: {responseMessage.StatusCode}");

        //var response = await httpClient.GetFromJsonAsync<GetInstrumentPositionBookResponse>(apiUrl);
        //logger.LogInformation($"Response: {response}");
        //System.Diagnostics.Debugger.Break();
    }


    // ACCOUNT ENDPOINTS

    public async Task GetAccountsAsync()
    {
        var responseMessage = await httpClient.GetAsync("/v3/accounts");

        GetAccountsPropertiesResponse? response = await httpClient.GetFromJsonAsync<GetAccountsPropertiesResponse>("/v3/accounts");

        if (response?.Accounts == null) return;

        //foreach (var account in response.Accounts)
        //{
        //    Console.WriteLine($"{account.Id}, {account.Mt4AccountID}");
        //}

        logger.LogInformation("Status code {StatusCode}", responseMessage.StatusCode);
    }

    public async Task GetAccountSummaryAsync(string? accountId)
    {
        if (accountId == null) return;

        string apiUrl = $"/v3/accounts/{accountId}/summary";

        var responseMessage = await httpClient.GetAsync(apiUrl);

        GetAccountSummaryResponse? response = await httpClient.GetFromJsonAsync<GetAccountSummaryResponse>(apiUrl);

        Console.WriteLine(response);

        Console.WriteLine($"Response status code: {responseMessage.StatusCode}");
    }

    public async Task<Instrument[]> GetAccountTradableInstrumentsAsync(string? accountId)
    {
        if (accountId == null) return Array.Empty<Instrument>();

        string apiUrl = $"/v3/accounts/{accountId}/instruments";

        var responseMessage = await httpClient.GetAsync(apiUrl);
        File.WriteAllText("tradable-instruments.json", await responseMessage.Content.ReadAsStringAsync());
        Console.WriteLine($"Response status code: {responseMessage.StatusCode}");

        var response = await httpClient.GetFromJsonAsync<GetAccountTradableInstrumentsResponse>(apiUrl);
        Console.WriteLine(response);

        if (response != null)
            return response.Instruments;

        return Array.Empty<Instrument>();
    }


    // INSTRUMENT ENDPOINTS

    public async Task GetInstrumentCandlesAsync(string? instrumentName)
    {
        if (instrumentName == null) return;

        string apiUrl = $"/v3/instruments/{instrumentName}/candles";

        var uriBuilder = new UriBuilder(httpClient.BaseAddress);
        uriBuilder.Path = apiUrl;

        var query = HttpUtility.ParseQueryString(string.Empty);
        query["price"] = "BMA";
        uriBuilder.Query = query.ToString();

        var responseMessage = await httpClient.GetAsync(uriBuilder.Uri);
        File.WriteAllText($"{instrumentName}-candles.json", await responseMessage.Content.ReadAsStringAsync());
        logger.LogInformation($"Response status code: {responseMessage.StatusCode}");

        var response = await httpClient.GetFromJsonAsync<GetInstrumentCandlesResponse>(uriBuilder.Uri);
        logger.LogInformation($"Response: {response}");
        //Console.WriteLine(response);
    }

    public async Task GetInstrumentOrderBookAsync(string? instrumentName)
    {
        if (instrumentName == null) return;

        string apiUrl = $"/v3/instruments/{instrumentName}/orderBook";

        //var responseMessage = await httpClient.GetAsync(apiUrl);
        //File.WriteAllText($"{instrumentName}-orderBook.json", await responseMessage.Content.ReadAsStringAsync());
        //logger.LogInformation($"Response status code: {responseMessage.StatusCode}");

        var response = await httpClient.GetFromJsonAsync<GetInstrumentOrderBookResponse>(apiUrl);
        logger.LogInformation($"Response: {response}");
        //System.Diagnostics.Debugger.Break();
    }

    public async Task GetInstrumentPositionBookAsync(string? instrumentName)
    {
        if (instrumentName == null) return;

        string apiUrl = $"/v3/instruments/{instrumentName}/positionBook";

        //var responseMessage = await httpClient.GetAsync(apiUrl);
        //File.WriteAllText($"{instrumentName}-positionBook.json", await responseMessage.Content.ReadAsStringAsync());
        //logger.LogInformation($"Response status code: {responseMessage.StatusCode}");

        var response = await httpClient.GetFromJsonAsync<GetInstrumentPositionBookResponse>(apiUrl);
        logger.LogInformation($"Response: {response}");
        System.Diagnostics.Debugger.Break();
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

