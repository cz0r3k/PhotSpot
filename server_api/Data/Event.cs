namespace server_api.Data;

public class Event
{
    public Guid Id { get; set; }
    public required User Owner { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public TimeSpan MinimalTimespan { get; set; }
    public required string QrCodePath { get; set; }
    public ICollection<Photo> Photos { get; set; } = new List<Photo>();
    public ICollection<LastUploadedPhoto> LastUploadedPhotos { get; set; } = new List<LastUploadedPhoto>();
}