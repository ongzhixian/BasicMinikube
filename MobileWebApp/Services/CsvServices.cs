using System.Text;

namespace MobileWebApp.Services;

public class CsvServices
{
    public static void ExportToCsv<T>(List<T> data, string filePath)
    {
        using var writer = new StreamWriter(filePath, false, Encoding.UTF8);

        // Get property names for header row
        var properties = typeof(T).GetProperties();
        var header = string.Join(",", properties.Select(p => p.Name));
        writer.WriteLine(header);

        // Write data rows
        foreach (var obj in data)
        {
            var row = string.Join(",", properties.Select(p =>
            {
                var value = p.GetValue(obj, null)?.ToString() ?? string.Empty;
                return $"\"{value.Replace("\"", "\"\"")}\""; // Escape double quotes within values
            }));

            writer.WriteLine(row);
        }
    }


    public static byte[] ExportToCsvBytes<T>(List<T> data)
    {
        using MemoryStream ms = new MemoryStream();
        using var writer = new StreamWriter(ms, Encoding.UTF8);

        // Get property names for header row
        var properties = typeof(T).GetProperties();
        var header = string.Join(",", properties.Select(p => p.Name));
        writer.WriteLine(header);

        // Write data rows
        foreach (var obj in data)
        {
            var row = string.Join(",", properties.Select(p =>
            {
                var value = p.GetValue(obj, null)?.ToString() ?? string.Empty;
                return $"\"{value.Replace("\"", "\"\"")}\""; // Escape double quotes within values
            }));

            writer.WriteLine(row);
        }

        writer.Close();
        ms.Close();
        return ms.ToArray();
    }

    public static List<T> ImportFromCsv<T>(string filePath)
    {
        var data = new List<T>();
        using (var reader = new StreamReader(filePath))
        {
            // Read header row
            var header = reader.ReadLine()?.Split(',').Select(h => h.Trim('"')).ToArray();
            if (header == null || header.Length == 0)
            {
                throw new InvalidDataException("Invalid CSV file: No header row found.");
            }

            // Get property names and their corresponding indices
            var properties = typeof(T).GetProperties();
            var propertyIndices = new Dictionary<string, int>();
            for (int i = 0; i < header.Length; i++)
            {
                propertyIndices[header[i]] = i;
            }

            // Read data rows
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var values = line.Split(',').Select(v => v.Trim('"')).ToArray();
                if (values.Length != header.Length)
                {
                    continue; // Skip invalid rows
                }

                var obj = Activator.CreateInstance<T>();
                for (int i = 0; i < properties.Length; i++)
                {
                    var propertyName = properties[i].Name;
                    if (propertyIndices.TryGetValue(propertyName, out var index))
                    {
                        try
                        {
                            var value = values[index];
                            var propertyType = properties[i].PropertyType;
                            var convertedValue = Convert.ChangeType(value, propertyType);
                            properties[i].SetValue(obj, convertedValue);
                        }
                        catch (Exception ex)
                        {
                            // Handle conversion errors (e.g., log, skip row)
                            Console.WriteLine($"Error converting value for property '{propertyName}': {ex.Message}");
                            // You can decide how to handle errors here (e.g., skip row, set default value)
                        }
                    }
                }

                data.Add(obj);
            }
        }

        return data;
    }
}
