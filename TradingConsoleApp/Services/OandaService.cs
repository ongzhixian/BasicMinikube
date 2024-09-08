﻿using System.Net.Http;
using System.Net.Http.Json;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TradingConsoleApp.Models.OandaApi;
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

    public async Task GetAccountTradableInstrumentsAsync(string? accountId)
    {
        if (accountId == null) return;

        string apiUrl = $"/v3/accounts/{accountId}/instruments";

        //var responseMessage = await httpClient.GetAsync(apiUrl);
        //File.WriteAllText("instruments.json", await responseMessage.Content.ReadAsStringAsync());
        //Console.WriteLine($"Response status code: {responseMessage.StatusCode}");

        var response = await httpClient.GetFromJsonAsync<GetAccountTradableInstrumentsResponse>(apiUrl);
        Console.WriteLine(response);
    }

    // INSTRUMENT ENDPOINTS

    public async Task GetInstrumentCandlesAsync(string? instrumentName)
    {
        if (instrumentName == null) return;

        string apiUrl = $"/v3/instruments/{instrumentName}/candles";

        //var responseMessage = await httpClient.GetAsync(apiUrl);
        //File.WriteAllText($"{instrumentName}-candles.json", await responseMessage.Content.ReadAsStringAsync());
        //logger.LogInformation($"Response status code: {responseMessage.StatusCode}");

        var response = await httpClient.GetFromJsonAsync<GetInstrumentCandlesResponse>(apiUrl);
        logger.LogInformation($"Response: {response}");
        //Console.WriteLine(response);
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
