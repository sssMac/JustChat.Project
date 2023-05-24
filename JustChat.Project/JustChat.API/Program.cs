using DAL.Data;
using JustChat.API.StartUp;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
//builder.WebHost.ConfigureKestrel(options =>
//{
//    // Setup a HTTP/2 endpoint without TLS.
//    options.ListenLocalhost(50051, o => o.Protocols =
//        HttpProtocols.Http2);
//});

builder.Services.RegisterService(builder.Configuration);
builder.Services.RegisterMongoDB(builder.Configuration);
builder.Services.RegisterRedisCache(builder.Configuration);
builder.Services.RegisterGrpc();

builder.Services.RegisterSignalR();
builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(
                    builder.Configuration["PostgreSQL:DefaultConnection"]));

var app = builder.Build();

app.ConfigureSwagger();
app.UseHttpsRedirection();
app.ConfigureSignalR();
app.ConfigureGrpc();
app.ConfigureCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
