using System.Text.Json.Serialization;
using System.Text.Json;

namespace TradingBot.Models;


public class PriceStreamDataJsonConverter : JsonConverter<PriceStreamData>
{
    //enum TypeDiscriminator
    //{
    //    Customer = 1,
    //    Employee = 2
    //}

    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(PriceStreamData).IsAssignableFrom(typeToConvert);
    }


    public override PriceStreamData ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return base.ReadAsPropertyName(ref reader, typeToConvert, options);
    }

    public override PricingHeartbeat Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        reader.Read();
        if (reader.TokenType != JsonTokenType.PropertyName)
        {
            throw new JsonException();
        }

        string? propertyName = reader.GetString();
        if (propertyName != "type")
        {
            throw new JsonException();
        }

        reader.Read();
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException();
        }

        //TypeDiscriminator typeDiscriminator = (TypeDiscriminator)reader.GetInt32();
        string typeDiscriminator = reader.GetString();
        PricingHeartbeat person = typeDiscriminator switch
        {
            "HEARTBEAT" => new PricingHeartbeat(),
            "PRICE" => new ClientPrice(),
            _ => throw new JsonException()
        };

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return person;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                propertyName = reader.GetString();
                reader.Read();
                switch (propertyName)
                {
                    //case "CreditLimit":
                    //    decimal creditLimit = reader.GetDecimal();
                    //    ((Customer)person).CreditLimit = creditLimit;
                    //    break;
                    //case "OfficeNumber":
                    //    string? officeNumber = reader.GetString();
                    //    ((Employee)person).OfficeNumber = officeNumber;
                    //    break;
                    case "Name":
                        string? name = reader.GetString();
                        person.type = "asd";
                        break;
                    default:
                        reader.GetString();
                        break;

                }
            }
        }

        throw new JsonException();
    }

    public override void Write(
        Utf8JsonWriter writer, PriceStreamData person, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        if (person is PricingHeartbeat customer)
        {
            //writer.WriteNumber("TypeDiscriminator", (int)TypeDiscriminator.Customer);
            //writer.WriteNumber("CreditLimit", customer.CreditLimit);
        }
        else if (person is ClientPrice employee)
        {
            //writer.WriteNumber("TypeDiscriminator", (int)TypeDiscriminator.Employee);
            //writer.WriteString("OfficeNumber", employee.OfficeNumber);
        }

        //writer.WriteString("Name", person);

        writer.WriteEndObject();
    }
}