using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BookingStatus
{
    [JsonPropertyName("reserved")]
    Reserved,

    [JsonPropertyName("active")]
    Active,

    [JsonPropertyName("finalized")]
    Finalized
}