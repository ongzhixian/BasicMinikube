namespace WareLogix.Models.OandaApiModels;

public record GetAccountTradableInstrumentsResponse
{
    public required Instrument[] Instruments { get; set; }

    public required string LastTransactionID { get; set; }
}
