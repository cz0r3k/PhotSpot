namespace server_api.Data;

public class Photo
{
    public Guid Id { get; set; }
    public required PhotoEvent PhotoEvent { get; set; }
    public required User User { get; set; }
    public string Path { get; set; }
    public DateTime UploadDate { get; set; }
    public bool IsDeleted { get; set; }
    public ICollection<PhotoLike> Likes { get; set; } = new List<PhotoLike>();
    public int LikesCount { get; set; }
}