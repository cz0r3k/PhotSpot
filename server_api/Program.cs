using server_api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();


builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
}));

var app = builder.Build();

app.UseGrpcWeb();
app.UseCors();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>().EnableGrpcWeb().RequireCors("AllowAll");
app.MapGet("/", () => "This gRPC service is gRPC-Web enabled, CORS enabled, and is callable from browser apps using the gRPC-Web protocol");
app.Run();
