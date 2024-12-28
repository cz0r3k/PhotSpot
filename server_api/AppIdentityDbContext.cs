using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using server_api.Identity;

namespace server_api;

internal class AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
    : IdentityDbContext<AppUser, AppRole, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        var adminRole = new AppRole
        {
            Id = new Guid("01940d8f-13eb-7747-ace7-fe90b7010875"),
            Name = "Admin",
            NormalizedName = "ADMIN",
            ConcurrencyStamp = null
        };
        builder.Entity<AppRole>().HasData(adminRole);
    }
}    