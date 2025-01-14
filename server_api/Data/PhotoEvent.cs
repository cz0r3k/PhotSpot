using System.ComponentModel.DataAnnotations;
using util.PhotoEvent;

namespace server_api.Data;

public class PhotoEvent
{
    public Guid Id { get; set; }
    [StringLength(maximumLength: 32)] public required string Name { get; set; }
    public required User Owner { get; set; }
    public required DateTime CreationDate { get; set; }
    public required DateTime ExpirationDate { get; set; }
    public required TimeSpan MinimalTimespan { get; set; }
    public required TimeSpan PhotoExpiration { get; set; }
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

    public PhotoEvent ToPhotoEvent(User user)
    {
        return new PhotoEvent
        {
            Name = Name, Owner = user, CreationDate = DateTime.Now,
            ExpirationDate = DateTime.Now.AddDays(1),
            MinimalTimespan = TimeSpan.FromHours(2), PhotoExpiration = TimeSpan.FromHours(4)
        };
    }
}