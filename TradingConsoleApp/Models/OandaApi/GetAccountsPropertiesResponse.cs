﻿namespace TradingConsoleApp.Models.OandaApi;

public record GetAccountsPropertiesResponse
{
    public required AccountProperties[] Accounts { get; set; }
}
