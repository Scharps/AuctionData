using System.Reflection;
using AuctionData.Application.Data;
using AuctionData.Application.Features;
using AuctionData.Application.Services.BlizzardApi;
using AuctionData.Application.Services.BlizzardApi.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<BlizzardAppOptions>(builder.Configuration.GetRequiredSection("BlizzardAppOptions"));
builder.Services.AddOAuthTokenManager(builder.Configuration);
builder.Services.AddBlizzardClient();
builder.Services.AddDbContext<AuctionDbContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("Default"), optionsBuilder => optionsBuilder.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddHostedService<ProcessAuctionData.ProcessAuctionDataHostedService>();

var app = builder.Build();

app.Run();