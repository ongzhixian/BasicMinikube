namespace TradingConsoleApp.Models.OandaApi;

public record AccountProperties
{
    public string Id { get; set; }

    public int Mt4AccountID { get; set; }

    public required string[] Tags { get; set; }
}
