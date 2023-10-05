using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionData.Infrastructure.BlizzardApi;

public static class ClientExtensions
{
    public static IServiceCollection AddBlizzardClient(this IServiceCollection services)
    {
        services.AddHttpClient<Client>(httpClient =>
        {
            httpClient.BaseAddress = new("https://eu.api.blizzard.com");
            httpClient.DefaultRequestVersion = HttpVersion.Version20;
        });
        return services;
    }
}