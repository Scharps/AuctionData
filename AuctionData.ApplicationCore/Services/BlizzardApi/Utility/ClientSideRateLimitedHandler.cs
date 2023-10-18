
using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Options;

namespace AuctionData.Application.Services.BlizzardApi.Utility;

//https://learn.microsoft.com/en-us/dotnet/core/extensions/http-ratelimiter

internal sealed class ClientSideRateLimitedHandler : DelegatingHandler, IAsyncDisposable
{
    private readonly RateLimiter _rateLimiter;

    public ClientSideRateLimitedHandler(IOptions<BlizzardAppOptions> options)
    {
        var appOptions = options.Value;
        var tokenBucketOptions = new TokenBucketRateLimiterOptions
        {
            TokenLimit = appOptions.TokenLimit,
            QueueProcessingOrder = Enum.Parse<QueueProcessingOrder>(appOptions.QueueProcessingOrder),
            QueueLimit = appOptions.QueueLimit,
            ReplenishmentPeriod = TimeSpan.FromMilliseconds(appOptions.ReplenishmentPeriod),
            TokensPerPeriod = appOptions.TokensPerPeriod,
            AutoReplenishment = appOptions.AutoReplenishment
        };
        _rateLimiter = new TokenBucketRateLimiter(tokenBucketOptions);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using RateLimitLease lease = await _rateLimiter.AcquireAsync(permitCount: 1, cancellationToken);

        if (lease.IsAcquired)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var response = new HttpResponseMessage(System.Net.HttpStatusCode.TooManyRequests);
        if (lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
        {
            response.Headers.Add(
                "Retry-After",
                ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo));
        }

        return response;
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await _rateLimiter.DisposeAsync().ConfigureAwait(false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _rateLimiter.Dispose();
        }
    }
}