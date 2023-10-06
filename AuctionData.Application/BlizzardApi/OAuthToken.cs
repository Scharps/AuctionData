using System.Text.Json.Serialization;

namespace AuctionData.Application.BlizzardApi;

/// <summary>
/// Todo: Move to Domain layer
/// </summary>
public sealed class OAuthToken
{
    public string AccessToken { get; }
    public string TokenType { get; }
    public DateTimeOffset Expiry { get; }
    public string Sub { get; }

    internal OAuthToken(string accessToken, string tokenType, int expiresIn, string sub)
    {
        AccessToken = accessToken;
        TokenType = tokenType;
        Sub = sub;
        Expiry = DateTime.Now + TimeSpan.FromSeconds(expiresIn);
    }

    public sealed class OAuthTokenDto
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; }

        [JsonPropertyName("sub")]
        public string Sub { get; }

        [JsonConstructor]
        public OAuthTokenDto(string accessToken, string tokenType, int expiresIn, string sub)
        {
            AccessToken = accessToken;
            TokenType = tokenType;
            ExpiresIn = expiresIn;
            Sub = sub;
        }

        public OAuthToken ToOAuthToken()
        {
            ArgumentException.ThrowIfNullOrEmpty(AccessToken);
            ArgumentException.ThrowIfNullOrEmpty(TokenType);
            ArgumentException.ThrowIfNullOrEmpty(Sub);
            if (0 == ExpiresIn)
                throw new ArgumentException("The token cannot expire in 0 seconds. Did the token serialize corrently?");

            return new(AccessToken, TokenType, ExpiresIn, Sub);
        }
    }
}