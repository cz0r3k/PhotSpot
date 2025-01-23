using Microsoft.EntityFrameworkCore;
using server_api.Data;

namespace server_api.Services;

public class PhotoService(
    ILogger<PhotoService> logger,
    AppDbContext appDbContext) : IPhotoService
{
    public async Task<bool> LikePhoto(string email, Guid eventId, Guid photoId)
    {
        var user = await appDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return false;
        var photo = await appDbContext.Photos.Include(p => p.PhotoEvent).Include(p => p.Likes)
            .ThenInclude(photoLike => photoLike.User).FirstOrDefaultAsync(p => p.Id == photoId && p.PhotoEvent.Id == eventId);
        if (photo == null) return false;
        if (photo.Likes.FirstOrDefault(l => l.User.Id == user.Id) != null) return false;
        photo.LikesCount += 1;
        appDbContext.PhotoLikes.Add(new PhotoLike { Photo = photo, User = user });
        await appDbContext.SaveChangesAsync();
        return true;
    }
}