using WareLogix.Models.OandaApiModels;

namespace TradingConsoleApp.Models.OandaApi;

public class GetAccountSummaryResponse
{
    public AccountSummary Account { get; set; }

    public string LastTransactionID { get; set; }
}
