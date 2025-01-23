using GrpcPhoto;

namespace util.Photo;

public class PhotoDetails
{
    public required int Likes { get; init; }
    public required bool IsUserLike { get; init; }

    public PhotoDetailsReply ToPhotoDetailsReply()
    {
        return new PhotoDetailsReply
        {
            Likes = Likes,
            IsUserLike = IsUserLike
        };
    }
}