using System.Diagnostics;
using System.Reflection;
using AuctionData.Application.BlizzardApi;
using AuctionData.Application.BlizzardApi.Extensions;
using AuctionData.Application.Data;
using AuctionData.Application.Services.BlizzardApi;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOAuthTokenManager(builder.Configuration);
builder.Services.AddBlizzardClient();
builder.Services.AddDbContext<AuctionDbContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("Default"), optionsBuilder => optionsBuilder.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();

using (var scope = app.Services.CreateAsyncScope())
{
    var client = scope.ServiceProvider.GetRequiredService<Client>();
    var data = await client.GetItemDetails(192791);
    System.Console.WriteLine(data);
    Debug.WriteLine(data);
}

app.Run();