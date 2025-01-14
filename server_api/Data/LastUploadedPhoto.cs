namespace server_api.Data;

public class LastUploadedPhoto
{
    public Guid Id { get; set; }
    public required User User { get; set; }
    public DateTime UploadDate { get; set; }
    public required PhotoEvent PhotoEvent { get; set; }
}