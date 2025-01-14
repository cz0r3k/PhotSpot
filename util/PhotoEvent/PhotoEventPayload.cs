using System.Text.Json;
using System.Text.Json.Serialization;

namespace util.PhotoEvent;

public class PhotoEventPayload
{
    [JsonPropertyName("ID")]
    [JsonConverter(typeof(GuidWithoutDashesConverter))]
    public required Guid EventId { get; init; }

    [JsonPropertyName("N")]
    public required string Name { get; init; }
    
    [JsonPropertyName("ED")]
    public required DateTime ExpirationDate { get; init; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}

public class GuidWithoutDashesConverter : JsonConverter<Guid>
{
    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Guid.Parse(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("N"));
    }
}