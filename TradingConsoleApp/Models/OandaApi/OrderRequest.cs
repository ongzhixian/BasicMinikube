namespace TradingConsoleApp.Models.OandaApi;





public class Order
{
    public string type { get; set; }

    public string units { get; set; }

    public string instrument { get; set; }

    public string timeInForce { get; set; } = "FOK";

    public string positionFill { get; set; } = "DEFAULT";
}




public class LimitOrder : Order
{
    public string price { get; set; }
    public Stoplossonfill stopLossOnFill { get; set; }
    public Takeprofitonfill takeProfitOnFill { get; set; }

    //public string timeInForce { get; set; }
    //public string instrument { get; set; }
    //public string units { get; set; }
    //public string type { get; set; }
    //public string positionFill { get; set; }
}

public class Stoplossonfill
{
    public string timeInForce { get; set; }
    public string price { get; set; }
}

public class Takeprofitonfill
{
    public string price { get; set; }
}
