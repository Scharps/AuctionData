using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionData.Infrastructure.BlizzardApi.Extensions;

public static class OAuthTokenManagerExtensions
{
    public static IServiceCollection AddOAuthTokenManager(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpClient<OAuthTokenManager>(client =>
        {
            var clientId = config["BlizzardAppCredentialOptions:ClientId"];
            var clientSecret = config["BlizzardAppCredentialOptions:ClientSecret"];

            var credentialData = Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}");
            var base64 = Convert.ToBase64String(credentialData);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64);

            client.BaseAddress = new("https://oauth.battle.net");
        });

        return services;
    }
}