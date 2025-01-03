namespace server_api.Data;

public class PhotoLike
{
    public Guid Id { get; set; }
    public required Photo Photo { get; set; }
    public required User User { get; set; }
}