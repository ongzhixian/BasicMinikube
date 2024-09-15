namespace WareLogix.Models.OandaApiModels;

// TODO: 
// 1. Make the fields Pascal-case
// 2. Make the fields to relevant types instead of strings
// Reference: https://developer.oanda.com/rest-live-v20/primitives-df/#AccountUnits
public class AccountSummary
{
    public string guaranteedStopLossOrderMode { get; set; }
    public bool hedgingEnabled { get; set; }
    public string id { get; set; }
    public string createdTime { get; set; }
    public string currency { get; set; }
    public int createdByUserID { get; set; }
    public string alias { get; set; }
    public string marginRate { get; set; }
    public string lastTransactionID { get; set; }
    public decimal Balance { get; set; }
    public int openTradeCount { get; set; }
    public int openPositionCount { get; set; }
    public int pendingOrderCount { get; set; }
    public string pl { get; set; }
    public string resettablePL { get; set; }
    public string resettablePLTime { get; set; }
    public string financing { get; set; }
    public string commission { get; set; }
    public string dividendAdjustment { get; set; }
    public string guaranteedExecutionFees { get; set; }
    public string unrealizedPL { get; set; }
    public string NAV { get; set; }
    public string marginUsed { get; set; }
    public string marginAvailable { get; set; }
    public string positionValue { get; set; }
    public string marginCloseoutUnrealizedPL { get; set; }
    public string marginCloseoutNAV { get; set; }
    public string marginCloseoutMarginUsed { get; set; }
    public string marginCloseoutPositionValue { get; set; }
    public string marginCloseoutPercent { get; set; }
    public string withdrawalLimit { get; set; }
    public string marginCallMarginUsed { get; set; }
    public string marginCallPercent { get; set; }
}
