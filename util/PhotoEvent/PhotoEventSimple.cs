using GrpcEvent;

namespace util.PhotoEvent;

public class PhotoEventSimple
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }

    public static PhotoEventSimple FromEventSimple(EventSimple eventSimple)
    {
        return new PhotoEventSimple
        {
            Id = Guid.Parse(eventSimple.Id.Value),
            Name = eventSimple.Name,
        };
    }

    public EventSimple ToEventSimple()
    {
        return new EventSimple
        {
            Id = new UUID { Value = Id.ToString() },
            Name = Name,
        };
    }
}