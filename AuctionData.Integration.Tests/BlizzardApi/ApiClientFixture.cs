using AuctionData.Application.Services.BlizzardApi;
using AuctionData.Application.Services.BlizzardApi.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Microsoft.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace AuctionData.Integration.Tests.BlizzardApi;

public class ApiClientFixture : TestBedFixture
{
    protected override void AddServices(IServiceCollection services, IConfiguration? configuration)
    {
        if (configuration is null) throw new NullReferenceException(nameof(configuration));
        services.Configure<BlizzardAppOptions>(configuration.GetRequiredSection("BlizzardAppOptions"));
        services.AddOAuthTokenManager(configuration);
        services.AddBlizzardClient();
    }

    protected override ValueTask DisposeAsyncCore() => new();

    protected override IEnumerable<TestAppSettings> GetTestAppSettings()
    {
        yield return new()
        {
            Filename = "appsettings.json",
            IsOptional = false
        };
    }
}