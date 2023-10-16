using System.Reflection;
using AuctionData.Application.Data;
using AuctionData.Application.Features;
using AuctionData.Application.Services.BlizzardApi;
using AuctionData.Application.Services.BlizzardApi.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<BlizzardAppOptions>(builder.Configuration.GetRequiredSection("BlizzardAppOptions"));
builder.Services.AddOAuthTokenManager(builder.Configuration);
builder.Services.AddBlizzardClient();
builder.Services.AddDbContext<AuctionDbContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("Default"), optionsBuilder => optionsBuilder.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();

using (var scope = app.Services.CreateAsyncScope())
{
    var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();
    await mediatr.Send(new ProcessAuctionData.ProcessAuctionDataCommand(1305));
    // var client = scope.ServiceProvider.GetRequiredService<Client>();
    // var data = await client.GetItemDetailsAsync(192791);
    // var d2 = await client.RequestConnectedRealmDataAsync(1305);
    // Debug.WriteLine(data);
}

app.Run();