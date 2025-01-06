using Microsoft.EntityFrameworkCore;
using QRCoder;
using server_api.Data;

namespace server_api.Services;

internal class PhotoEventService(
    ILogger<UserManagementService> logger,
    AppDbContext appDbContext) : IPhotoEventService
{
    public async Task<bool> Create(string email, PhotoEventArgs photoEventArgs)
    {
        var user = await appDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return false;
        var photoEvent = photoEventArgs.ToPhotoEvent(user);
        await appDbContext.Events.AddAsync(photoEvent);
        var qrGenerator = new QRCodeGenerator();
        var eventPayload = photoEvent.ToPhotoEventPayload();
        var qrCodeData = qrGenerator.CreateQrCode(eventPayload.ToString(), QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrCodeData);
        await File.WriteAllBytesAsync($"events/{photoEvent.Id}.png", qrCode.GetGraphic(20));
        logger.LogInformation($"Event {photoEvent.Id} created");
        return true;
    }
}