using System.Net;
using AuctionData.Application.Services.BlizzardApi.Utility;

namespace AuctionData.Application.Services.BlizzardApi.Extensions;

public static class ClientExtensions
{
    public static IServiceCollection AddBlizzardClient(this IServiceCollection services)
    {
        services
        .AddTransient<OAuthCredentialHandler>()
        .AddTransient<ClientSideRateLimitedHandler>()
        .AddHttpClient<Client>(httpClient =>
        {
            httpClient.BaseAddress = new("https://eu.api.blizzard.com");
            httpClient.DefaultRequestVersion = HttpVersion.Version20;
        })
        .AddHttpMessageHandler<OAuthCredentialHandler>()
        .AddHttpMessageHandler<ClientSideRateLimitedHandler>();
        return services;
    }
}