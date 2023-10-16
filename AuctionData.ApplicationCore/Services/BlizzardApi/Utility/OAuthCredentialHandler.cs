using System.Net.Http.Headers;
using AuctionData.Application.Services.BlizzardApi;

internal sealed class OAuthCredentialHandler : DelegatingHandler
{
    private readonly OAuthTokenManager _tokenManager;

    public OAuthCredentialHandler(OAuthTokenManager tokenManager)
    {
        _tokenManager = tokenManager;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenManager.RequestToken();

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        return await base.SendAsync(request, cancellationToken);
    }
}