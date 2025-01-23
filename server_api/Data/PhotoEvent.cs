using System.ComponentModel.DataAnnotations;
using util.PhotoEvent;

namespace server_api.Data;

public class PhotoEvent
{
    public Guid Id { get; set; }
    [StringLength(maximumLength: 32)]
    public required string Name { get; set; }
    public required User Owner { get; set; }
    public required DateTime CreationDate { get; set; }
    public required DateTime ExpirationDate { get; set; }
    public required TimeSpan MinimalTimespan { get; set; }
    public required TimeSpan PhotoExpiration { get; set; }
    public decimal? Latitude  { get; set; }
    public decimal? Longitude   { get; set; }
    public ICollection<Photo> Photos { get; } = new List<Photo>();
    public ICollection<LastUploadedPhoto> LastUploadedPhotos { get; } = new List<LastUploadedPhoto>();

    public PhotoEventPayload ToPhotoEventPayload()
    {
        return new PhotoEventPayload { EventId = Id, Name = Name, ExpirationDate = ExpirationDate };
    }

    public PhotoEventDetails ToPhotoEventDetails()
    {
        return new PhotoEventDetails
        {
            Id = Id,
            Name = Name,
            ExpirationDate = ExpirationDate,
            Owner = Owner.Id,
            CreationDate = CreationDate,
            MinimalTimespan = MinimalTimespan,
            PhotoExpiration = PhotoExpiration
        };
    }

    public PhotoEventSimple ToPhotoEventSimple()
    {
        return new PhotoEventSimple
        {
            Id = Id,
            Name = Name,
        };
    }
}

public class PhotoEventArgs
{
    public required string Name { get; init; }
    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }

    public PhotoEvent ToPhotoEvent(User user)
    {
        var now = DateTime.Now;
        now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
        return new PhotoEvent
        {
            Name = Name, Owner = user, CreationDate = now,
            ExpirationDate = now.AddDays(1),
            MinimalTimespan = TimeSpan.FromHours(2), PhotoExpiration = TimeSpan.FromHours(4),
            Latitude = Latitude, Longitude = Longitude
        };
    }
}