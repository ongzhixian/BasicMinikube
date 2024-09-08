namespace TradingConsoleApp.Models.OandaApi;


public class GetInstrumentPositionBookResponse
{
    public Positionbook positionBook { get; set; }
}

public class Positionbook
{
    public string instrument { get; set; }
    public DateTime time { get; set; }
    public string unixTime { get; set; }
    public string price { get; set; }
    public string bucketWidth { get; set; }
    public Bucket[] buckets { get; set; }
}


