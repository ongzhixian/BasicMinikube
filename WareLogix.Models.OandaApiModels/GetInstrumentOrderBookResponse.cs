namespace WareLogix.Models.OandaApiModels;

// TODO: Make nice nice

public class GetInstrumentOrderBookResponse
{
    public Orderbook orderBook { get; set; }
}

public class Orderbook
{
    public string instrument { get; set; }
    public DateTime time { get; set; }
    public string unixTime { get; set; }
    public string price { get; set; }
    public string bucketWidth { get; set; }
    public Bucket[] buckets { get; set; }
}


