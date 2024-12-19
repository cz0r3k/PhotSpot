using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace server_api;

internal class AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
    : IdentityDbContext<AppUser, IdentityRole, string>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        var adminRole = new IdentityRole
        {
            Id = "9f68937c-dff0-4e17-96e5-d16254df9f24",
            Name = "Admin",
            NormalizedName = "ADMIN",
            ConcurrencyStamp = null
        };
        builder.Entity<IdentityRole>().HasData(adminRole);
    }
}    