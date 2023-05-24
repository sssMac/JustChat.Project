using DAL.Data;
using JustChat.API.StartUp;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using JustChat.API.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterService(builder.Configuration);
builder.Services.RegisterMongoDB(builder.Configuration);
builder.Services.RegisterRedisCache(builder.Configuration);
builder.Services.RegisterCors();
builder.Services.RegisterGrpc();

builder.Services.RegisterSignalR();
builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(
                    builder.Configuration["PostgreSQL:DefaultConnection"]));

var app = builder.Build();

app.UseRouting();
app.ConfigureSwagger();
app.UseHttpsRedirection();
app.ConfigureSignalR();
app.ConfigureCors();
app.ConfigureGrpc();
app.UseAuthorization();
app.MapControllers();

app.Run();
