using System.Text.Json;
using System;
using System.Text.Json.Serialization;

namespace TradingBot.Models;

//[JsonDerivedType(typeof(PriceStreamRecord), typeDiscriminator: "heartBeat")]
//[JsonDerivedType(typeof(ClientPrice), typeDiscriminator: "withCity")]


//[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
//[JsonDerivedType(typeof(PricingHeartbeat), typeDiscriminator: "HEARTBEAT")]
//[JsonDerivedType(typeof(ClientPrice), typeDiscriminator: "PRICE")]

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(PricingHeartbeat), typeDiscriminator: "HEARTBEAT")]
[JsonDerivedType(typeof(ClientPrice), typeDiscriminator: "PRICE")]
public record PriceStreamData
{
    public string type { get; set; }
}


public record PricingHeartbeat : PriceStreamData
{
    //public string type { get; set; }

    public string time { get; set; }

}

public record ClientPrice : PricingHeartbeat
{
    //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Bid[]? bids { get; set; }

    //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Ask[]? asks { get; set; }

    //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? closeoutBid { get; set; }

    //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? closeoutAsk { get; set; }

    //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? status { get; set; }

    //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? tradeable { get; set; }

    //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? instrument { get; set; }

}

public class Bid
{
    public string price { get; set; }
    public int liquidity { get; set; }
}

public class Ask
{
    public string price { get; set; }
    public int liquidity { get; set; }
}

