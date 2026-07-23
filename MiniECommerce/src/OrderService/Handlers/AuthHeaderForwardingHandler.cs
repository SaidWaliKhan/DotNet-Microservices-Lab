using System.Net.Http.Headers;

namespace OrderService.Handlers;

// Product Service and User Service now require a valid JWT on their
// protected endpoints. When Order Service calls them on behalf of the
// logged-in user, it needs to forward that SAME token - otherwise every
// downstream call gets rejected with a 401, even though the original
// caller was authenticated.
public class AuthHeaderForwardingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthHeaderForwardingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var incomingAuthHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();

        if (!string.IsNullOrEmpty(incomingAuthHeader) &&
            AuthenticationHeaderValue.TryParse(incomingAuthHeader, out var authHeader))
        {
            request.Headers.Authorization = authHeader;
        }

        return base.SendAsync(request, cancellationToken);
    }
}