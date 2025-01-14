using QRCoder;

namespace util.PhotoEvent;

public class PhotoEventPayload : PayloadGenerator.Payload
{
    public Guid EventId { get; init; }
    public required string Name { get; init; }
    public DateTime ExpirationDate { get; init; }

    public override string ToString()
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }
}