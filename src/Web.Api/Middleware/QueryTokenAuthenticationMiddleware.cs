namespace Web.Api.Middleware;

public class QueryTokenAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<QueryTokenAuthenticationMiddleware> _logger;
    private const string AccessTokenQueryParam = "access_token";
    private const string AuthorizationHeader = "Authorization";
    private const string BearerPrefix = "Bearer ";

    public QueryTokenAuthenticationMiddleware(
        RequestDelegate next,
        ILogger<QueryTokenAuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public Task Invoke(HttpContext context)
    {
        if (context.Request.Headers.ContainsKey(AuthorizationHeader))
        {
            return _next(context);
        }

        if (context.Request.Query.TryGetValue(AccessTokenQueryParam, out var token) &&
            !string.IsNullOrWhiteSpace(token))
        {
            context.Request.Headers[AuthorizationHeader] = BearerPrefix + token;
            _logger.LogDebug("توکن از پارامتر کوئری به هدر Authorization منتقل شد برای مسیر: {Path}", context.Request.Path);
        }
        else
        {
            _logger.LogDebug("پارامتر access_token در کوئری استرینگ یافت نشد برای مسیر: {Path}", context.Request.Path);
        }

        return _next(context);
    }
}

