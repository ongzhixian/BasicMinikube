namespace WareLogix.Models.OandaApiModels;

// TODO: Make nice nice
public class Instrument
{
    public string name { get; set; }
    public string type { get; set; }
    public string displayName { get; set; }
    public int pipLocation { get; set; }
    public int displayPrecision { get; set; }
    public int tradeUnitsPrecision { get; set; }
    public string minimumTradeSize { get; set; }
    public string maximumTrailingStopDistance { get; set; }
    public string minimumTrailingStopDistance { get; set; }
    public string maximumPositionSize { get; set; }
    public string maximumOrderUnits { get; set; }
    public string marginRate { get; set; }
    public string guaranteedStopLossOrderMode { get; set; }
    public Tag[] tags { get; set; }
    public Financing financing { get; set; }
    public string minimumGuaranteedStopLossDistance { get; set; }
    public string guaranteedStopLossOrderExecutionPremium { get; set; }
    public Guaranteedstoplossorderlevelrestriction guaranteedStopLossOrderLevelRestriction { get; set; }
}

public class Financing
{
    public string longRate { get; set; }
    public string shortRate { get; set; }
    public Financingdaysofweek[] financingDaysOfWeek { get; set; }
}

public class Financingdaysofweek
{
    public string dayOfWeek { get; set; }
    public int daysCharged { get; set; }
}

public class Guaranteedstoplossorderlevelrestriction
{
    public string volume { get; set; }

    public string priceRange { get; set; }
}

public class Tag
{
    public required string Type { get; set; }

    public required string Name { get; set; }
}
