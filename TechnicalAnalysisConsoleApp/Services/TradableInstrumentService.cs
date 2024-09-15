using System.Collections.ObjectModel;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace TechnicalAnalysisConsoleApp.Services;

internal class TradableInstrumentService
{
    // ILogger<CloudAmqpDemoService> logger, IMessageQueueConsumer<string> messageQueueConsumer

    private readonly ILogger<TradableInstrumentService> logger;

    private readonly string databaseFilePath = "C:\\src\\github.com\\ongzhixian\\BasicFlaskApp\\instance\\webapp.sqlite";

    public TradableInstrumentService(ILogger<TradableInstrumentService> logger)
    {
        this.logger = logger;
    }

    public Dictionary<string, long> GetOandaIntrumentTypes()
    {
        //select* from oanda_intrument_type

        using var connection = new SqliteConnection($"Data Source={databaseFilePath}");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"SELECT code, id FROM oanda_intrument_type;";

        var valuePairs = new Dictionary<string, long>();

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var code = (string)reader["code"];
            var type_id = (long)reader["id"];

            if (valuePairs.ContainsKey(code)) continue;

            valuePairs.Add(code, type_id);
        }

        return valuePairs;
    }

    public long AddOandaInstrument(string name, long instrument_type_id, string display_name)
    {
        // C:\src\github.com\ongzhixian\BasicFlaskApp\instance\webapp.sqlite
        //string databaseFilePath = "C:\\src\\github.com\\ongzhixian\\BasicFlaskApp\\instance\\webapp.sqlite";
        using var connection = new SqliteConnection($"Data Source={databaseFilePath}");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
        @"
INSERT OR REPLACE INTO oanda_intrument (name, instrument_type_id, display_name)
VALUES ($name, $instrument_type_id, $display_name)
RETURNING 
    id AS instrument_id;
";
        command.Parameters.AddWithValue("$name", name);
        command.Parameters.AddWithValue("$instrument_type_id", instrument_type_id);
        command.Parameters.AddWithValue("$display_name", display_name);

        var reader = command.ExecuteReader();

        if (!reader.Read()) return 0;

        return (long)reader["instrument_id"];
        

        //using (var reader = command.ExecuteReader())
        //{
        //    while (reader.Read())
        //    {
        //        var name = reader.GetString(0);

        //        Console.WriteLine($"Hello, {name}!");
        //    }
        //}
    }

    public void AddOandaInstrumentTag(long instrument_id, string tagName, string tagValue)
    {
        using var connection = new SqliteConnection($"Data Source={databaseFilePath}");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
        @"
INSERT OR REPLACE INTO oanda_intrument_tag (instrument_id, name, value) 
VALUES ($instrument_id, $name, $value);
";
        
        command.Parameters.AddWithValue("$instrument_id", instrument_id);
        command.Parameters.AddWithValue("$name", tagName);
        command.Parameters.AddWithValue("$value", tagValue);

        var recordedAffected = command.ExecuteNonQuery();
    }

    public IEnumerable<TradableInstrument> GetTradableInstruments()
    {
        using var connection = new SqliteConnection($"Data Source={databaseFilePath}");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
SELECT 	instr.name 'instrument_code'
		, instr.display_name 'instrument_name'
		, it.code 'instrument_type'
		, tag.name 'asset_class_type'
		, tag.value 'asset_class_value'
FROM 	oanda_intrument instr
INNER JOIN oanda_intrument_type it
		ON it.id = instr.instrument_type_id
INNER JOIN oanda_intrument_tag tag
		ON instr.id = tag.instrument_id
WHERE 	tag.name LIKE 'ASSET_CLASS%'
ORDER BY instr.instrument_type_id, instr.name;";

        ICollection<TradableInstrument> TradableInstruments = new Collection<TradableInstrument>();

        using var reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            var rec = new TradableInstrument
            {
                instrument_code = (string)reader["instrument_code"],
                instrument_name = (string)reader["instrument_name"],
                instrument_type = (string)reader["instrument_type"],
                asset_class_type = (string)reader["asset_class_type"],
                asset_class_value = (string)reader["asset_class_value"]
            };

            TradableInstruments.Add(rec);
        }

        return TradableInstruments;
    }


    public IEnumerable<TradableInstrumentSummaryItem> GetTradableInstrumentSummary()
    {
        using var connection = new SqliteConnection($"Data Source={databaseFilePath}");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
SELECT 	it.code 'instrument_type'
		, tag.value 'asset_class_value'
		, COUNT(instr.id) 'instrument_count'
FROM 	oanda_intrument instr
INNER JOIN oanda_intrument_type it
		ON it.id = instr.instrument_type_id
INNER JOIN oanda_intrument_tag tag
		ON instr.id = tag.instrument_id
WHERE 	tag.name = 'ASSET_CLASS'
GROUP BY it.code, tag.value
ORDER BY it.code, tag.value;";

        IList<TradableInstrumentSummaryItem> TradableInstruments = 
            new List<TradableInstrumentSummaryItem>();

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var rec = new TradableInstrumentSummaryItem
            {
                instrument_type = (string)reader["instrument_type"],
                asset_class_value = (string)reader["asset_class_value"],
                instrument_count = (long)reader["instrument_count"]
            };

            TradableInstruments.Add(rec);
        }

        return TradableInstruments;
    }
}


public record TradableInstrumentSummaryItem
{

    public string instrument_type { get; init; } = null!;
    
    public string asset_class_value { get; init; } = null!;

    public long instrument_count { get; init; }

}

public record TradableInstrument
{
    public string instrument_code { get; set; } = null!;
    public string instrument_name { get; set; } = null!;
    public string instrument_type { get; set; } = null!;
    public string asset_class_type { get; set; } = null!;
    public string asset_class_value { get; init; } = null!;

}