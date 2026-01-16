using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using System.Net;

namespace ExamPlatform.API.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private readonly ILogger<RateLimitingMiddleware> _logger;

    public RateLimitingMiddleware(RequestDelegate next, IMemoryCache cache, ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _cache = cache;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var endpoint = context.Request.Path.ToString().ToLower();
        
        // Strict limit for auth
        int limit = endpoint.Contains("/auth/") ? 10 : 100; // 10 req/min for auth, 100 for others
        var cacheKey = $"rate_limit_{ipAddress}_{endpoint}";

        if (!_cache.TryGetValue(cacheKey, out int requestCount))
        {
            requestCount = 0;
        }

        requestCount++;

        if (requestCount > limit)
        {
             _logger.LogWarning("Rate limit exceeded for IP: {IP} on {Endpoint}", ipAddress, endpoint);
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
            return;
        }

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(1));

        _cache.Set(cacheKey, requestCount, cacheEntryOptions);

        await _next(context);
    }
}
