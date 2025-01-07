using Microsoft.EntityFrameworkCore;
using QRCoder;
using server_api.Data;
using util.PhotoEvent;
using PhotoEvent = server_api.Data.PhotoEvent;

namespace server_api.Services;

internal class PhotoEventService(
    ILogger<UserManagementService> logger,
    AppDbContext appDbContext) : IPhotoEventService
{
    public async Task<Guid?> Create(string email, PhotoEventArgs photoEventArgs)
    {
        var user = await appDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null) return null;
        var photoEvent = photoEventArgs.ToPhotoEvent(user);
        await appDbContext.Events.AddAsync(photoEvent);
        await appDbContext.SaveChangesAsync();
        await CreateQrCode(photoEvent.ToPhotoEventPayload());
        logger.LogInformation($"Event {photoEvent.Id} created");
        return photoEvent.Id;
    }

    public Task<PhotoEvent?> GetDetails(Guid id)
    {
        var photoEvent = appDbContext.Events.Include(e => e.Owner).FirstOrDefaultAsync(e => e.Id == id);
        return photoEvent;
    }

    public async Task<IEnumerable<PhotoEventSimple>> GetActiveEvents()
    {
        var photoEvents =
            (await appDbContext.Events.Where(e => e.ExpirationDate > DateTime.Now).ToListAsync()).Select(e =>
                e.ToPhotoEventSimple());
        return photoEvents;
    }

    private static async Task CreateQrCode(PhotoEventPayload photoEventPayload)
    {
        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(photoEventPayload.ToString(), QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrCodeData);
        await File.WriteAllBytesAsync($"events/{photoEventPayload.EventId}.png", qrCode.GetGraphic(20));
    }
}