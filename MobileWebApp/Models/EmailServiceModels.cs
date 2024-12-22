using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MobileWebApp.Models;

/// <summary>
/// This is an incomplete implementation of MailerSend email models
/// https://developers.mailersend.com/api/v1/email.html
/// </summary>
public class EmailMessageModel
{
    private readonly JsonSerializerOptions jsonSerializerOptions;

    [JsonPropertyName("from")]
    public Email Sender { get; set; } = null!;

    [JsonPropertyName("to")]
    public Email[] Recipients { get; set; } = null!;

    [JsonPropertyName("subject")]
    public string Subject { get; set; } = null!;

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("html")]
    public string? Html { get; set; }

    [JsonPropertyName("personalization")]
    public Personalization[]? personalization { get; set; }

    public EmailMessageModel()
    {
        jsonSerializerOptions = new(JsonSerializerDefaults.Web);
        jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this, jsonSerializerOptions);
    }

    public StringContent ToStringContent()
    {
        return new StringContent(
            ToString(),
            Encoding.UTF8,
            MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json));
    }
}

public class Email
{
    [JsonPropertyName("email")]
    public required string Address { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}


public class Personalization
{
    [JsonPropertyName("email")]
    public required string EmailAddress { get; set; }

    [JsonPropertyName("data")]
    public KeyValuePair<string, string> Data { get; set; }
}
