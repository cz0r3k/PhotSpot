using System.Globalization;
using GrpcEvent;

namespace util.PhotoEvent;

public class PhotoEventLocation
{
    public required string Name { get; init; }
    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }

    public static PhotoEventLocation FromEventLocation(EventLocation eventLocation)
    {
        return new PhotoEventLocation
        {
            Name = eventLocation.Name,
            Latitude = decimal.Parse(eventLocation.Location.Latitude),
            Longitude = decimal.Parse(eventLocation.Location.Longitude),
        };
    }

    public EventLocation ToEventLocation()
    {
        return new EventLocation
        {
            Name = Name,
            Location = new Coordinates
            {
                Latitude = Latitude?.ToString(CultureInfo.InvariantCulture),
                Longitude = Longitude?.ToString(CultureInfo.InvariantCulture)
            }
        };
    }
}