using System.Reflection;
using AuctionData.Application.BlizzardApi;
using AuctionData.Application.BlizzardApi.Extensions;
using AuctionData.Application.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOAuthTokenManager(builder.Configuration);
builder.Services.AddBlizzardClient();
builder.Services.AddDbContext<AuctionDbContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("Default"), optionsBuilder => optionsBuilder.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();

// using (var scope = app.Services.CreateAsyncScope())
// {
//     var client = scope.ServiceProvider.GetRequiredService<Client>();
//     var dbContext = scope.ServiceProvider.GetRequiredService<AuctionContext>();
//     var data = await client.RequestConnectedRealmData(1305);
//     await dbContext.Auctions.AddRangeAsync(data);
//     await dbContext.SaveChangesAsync();
// }

app.Run();