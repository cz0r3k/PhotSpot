using Microsoft.EntityFrameworkCore;
using server_api.Data;

namespace server_api;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Event> Events { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<LastUploadedPhoto> LastUploadedPhotos { get; set; }
    public DbSet<PhotoLike> PhotoLikes { get; set; }
}