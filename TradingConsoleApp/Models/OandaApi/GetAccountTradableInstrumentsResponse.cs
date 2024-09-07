namespace TradingConsoleApp.Models.OandaApi;

public class GetAccountTradableInstrumentsResponse
{
    public Instrument[] Instruments { get; set; }

    public string LastTransactionID { get; set; }
}
