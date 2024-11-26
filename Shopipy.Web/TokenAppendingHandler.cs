using System.Net.Http.Headers;

namespace Shopipy.Web;

public class TokenAppendingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenAppendingHandler(IHttpContextAccessor httpContextAccessor, HttpMessageHandler innerHandler)
    {
        _httpContextAccessor = httpContextAccessor;
        InnerHandler = innerHandler;
    }

    protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext;

        if (context == null)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var token = context.Request.Cookies["BearerToken"];

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}