using System.ComponentModel.DataAnnotations;
using QRCoder;

namespace server_api.Data;

public class PhotoEvent
{
    public Guid Id { get; set; }
    [StringLength(maximumLength: 32)] public required string Name { get; set; }
    public required User Owner { get; set; }
    public DateTime CreationDate { get; } = DateTime.Now;
    public required DateTime ExpirationDate { get; set; }
    public required TimeSpan MinimalTimespan { get; set; }
    public ICollection<Photo> Photos { get; } = new List<Photo>();
    public ICollection<LastUploadedPhoto> LastUploadedPhotos { get; } = new List<LastUploadedPhoto>();

    public PhotoEventPayload ToPhotoEventPayload()
    {
        return new PhotoEventPayload { EventId = Id, Name = Name, ExpirationDate = ExpirationDate };
    }
}

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

public class PhotoEventArgs
{
    public required string Name { get; init; }

    public PhotoEvent ToPhotoEvent(User user)
    {
        return new PhotoEvent
        {
            Name = Name, Owner = user, ExpirationDate = DateTime.Today.AddDays(1),
            MinimalTimespan = TimeSpan.FromHours(2)
        };
    }
}