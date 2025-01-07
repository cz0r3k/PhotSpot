using Google.Protobuf.WellKnownTypes;
using GrpcEvent;

namespace util.PhotoEvent;

public class PhotoEventDetails
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required Guid Owner { get; set; }
    public required DateTime CreationDate { get; set; }
    public required DateTime ExpirationDate { get; set; }
    public required TimeSpan MinimalTimespan { get; set; }
    public required TimeSpan PhotoExpiration {get; set;}
    
    public static PhotoEventDetails FromDetailsReply(DetailsReply detailsReply)
    {
        return new PhotoEventDetails
        {
            Id = Guid.Parse(detailsReply.Details.Id.Value),
            Name = detailsReply.Details.Name,
            Owner = Guid.Parse(detailsReply.Details.Owner.Value),
            CreationDate = detailsReply.Details.CreationDate.ToDateTime(),
            ExpirationDate = detailsReply.Details.ExpirationDate.ToDateTime(),
            MinimalTimespan = detailsReply.Details.MinimalTimespan.ToTimeSpan(),
            PhotoExpiration = detailsReply.Details.PhotoExpiration.ToTimeSpan(),
        };
    }
    
    public DetailsReply ToDetailsReply()
    {
        return new DetailsReply
        {
            Details = new EventDetails
            {
                Id = new UUID { Value = Id.ToString() },
                Name = Name,
                CreationDate = Timestamp.FromDateTimeOffset(CreationDate.ToLocalTime()),
                ExpirationDate = Timestamp.FromDateTimeOffset(ExpirationDate.ToLocalTime()),
                MinimalTimespan = Duration.FromTimeSpan(MinimalTimespan),
                PhotoExpiration = Duration.FromTimeSpan(PhotoExpiration),
                Owner = new UUID { Value = Owner.ToString() },
            }
        };
    }
}