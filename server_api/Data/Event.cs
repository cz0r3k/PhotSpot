using System.ComponentModel.DataAnnotations;
using QRCoder;

namespace server_api.Data;

public class Event
{
    public Guid Id { get; set; }
    [StringLength(maximumLength:32)]
    public required string Name { get; set; }
    public required User Owner { get; set; }
    public DateTime CreationDate { get; } = DateTime.Now;
    public DateTime ExpirationDate { get; set; }
    public TimeSpan MinimalTimespan { get; set; }
    public ICollection<Photo> Photos { get; } = new List<Photo>();
    public ICollection<LastUploadedPhoto> LastUploadedPhotos { get; } = new List<LastUploadedPhoto>();
}

public class EventPayload : PayloadGenerator.Payload
{
    public Guid EventId { get; init; }
    public DateTime ExpirationDate { get; init; }
    public override string ToString()
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }
}