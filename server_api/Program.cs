using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using server_api;
using server_api.Identity;
using server_api.Services;
using server_api.Services.QrManager;
using server_api.ServicesGRPC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppIdentityDbContext>(x =>
    x.UseSqlite("DataSource=Database/appIdentity.db")
);
builder.Services.AddDbContext<AppDbContext>(x =>
    x.UseSqlite("DataSource=Database/main.db")
);

builder.Services.AddIdentityCore<AppUser>(o => o.User.RequireUniqueEmail = true)
    .AddRoles<AppRole>()
    .AddEntityFrameworkStores<AppIdentityDbContext>();

builder.Services.AddGrpc();

var r = new StreamReader("GoogleSigningKeys.json");
var googleSigningKey = r.ReadToEnd();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKeys = new JsonWebKeySet(googleSigningKey).Keys,
        ValidAudience = builder.Configuration["Authentication:Google:ClientId"]!,
        ValidIssuer = "https://accounts.google.com",
    };
});
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(o => o.AddPolicy("AllowAll", corsPolicyBuilder =>
{
    corsPolicyBuilder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
}));

builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IPhotoEventService, PhotoEventService>();
//builder.Services.AddScoped<IQrManager, QrManagerFile>();
builder.Services.AddScoped<IQrManager, QrManagerBlob>();
builder.Services.AddSingleton(_ => new BlobServiceClient("UseDevelopmentStorage=true"));


var app = builder.Build();

app.UseGrpcWeb();
app.UseCors();
app.UseMiddleware<RoleClaimsMiddleware>();
app.UseAuthorization();

app.MapGrpcService<PhotosAService>().AllowAnonymous();
app.MapGrpcService<GreeterService>().EnableGrpcWeb().RequireCors("AllowAll");
app.MapGrpcService<UserManagementService>().EnableGrpcWeb().RequireCors("AllowAll");
app.MapGrpcService<PhotoEventServiceGrpc>().EnableGrpcWeb().RequireCors("AllowAll");

app.MapGet("/",
    () =>
        "This gRPC service is gRPC-Web enabled, CORS enabled, and is callable from browser apps using the gRPC-Web protocol");


app.Run();