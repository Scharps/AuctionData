using System.Text.Json;
using static AuctionData.Application.Services.BlizzardApi.OAuthToken;

namespace AuctionData.Application.Services.BlizzardApi;

public sealed class OAuthTokenManager
{
    private readonly HttpClient _httpClient;
    private OAuthToken? _oAuthToken;

    private readonly FormUrlEncodedContent _content = new(new[]
    {
        new KeyValuePair<string, string>("grant_type", "client_credentials")
    });

    public OAuthTokenManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary> 
    /// Requests an <see cref="OAuthToken">, if the last requested token has not yet expired, that token will be returned.
    /// This behaviour can be overidden with <paramref name="forceRequest"/>.
    /// </summary>
    /// <param name="forceRequest">force a request for a new token</param>
    /// <returns>A <see cref="Task"> representing this process.</returns>
    public async Task<OAuthToken> RequestToken(bool forceRequest = false)
    {
        // If forced, if the token as expired, or if the token has not been initialised, request a new token.
        if (forceRequest || TokenExpired())
        {
            if (_httpClient is null)
                throw new NullReferenceException("Unable to request token because the HTTP client is unavailble.");

            var result = await _httpClient.PostAsync("/token", _content);
            result.EnsureSuccessStatusCode();
            var dto = await result.Content.ReadFromJsonAsync<OAuthTokenDto>();
            _oAuthToken = dto!.ToOAuthToken();
        }
        // Else, return the token
        return _oAuthToken!;
    }

    private bool TokenExpired() => _oAuthToken == null || _oAuthToken.Expiry < DateTime.Now;
}
