using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using server_api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

var r = new StreamReader("GoogleSigningKeys.json");
var googleSigningKey = r.ReadToEnd();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = (new JsonWebKeySet(googleSigningKey)).Keys.First(),
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

var app = builder.Build();

app.UseGrpcWeb();
app.UseCors();

app.MapGrpcService<GreeterService>().EnableGrpcWeb().RequireCors("AllowAll");
app.MapGet("/",
    () =>
        "This gRPC service is gRPC-Web enabled, CORS enabled, and is callable from browser apps using the gRPC-Web protocol");
app.Run();