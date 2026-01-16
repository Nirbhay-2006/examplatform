using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using ExamPlatform.API.Services;

namespace ExamPlatform.API.Middleware;

public class SessionMonitorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SessionMonitorMiddleware> _logger;

    public SessionMonitorMiddleware(RequestDelegate next, ILogger<SessionMonitorMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IAntiCheatService antiCheatService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentIp = context.Connection.RemoteIpAddress?.ToString();
            var userAgent = context.Request.Headers["User-Agent"].ToString();

            // In a real strict implementation, we would store the initial login IP/UA in Redis or DB
            // and compare it here. For now, we will just log it and potentially flag if it changes mid-session 
            // (requires session state which we don't have yet, so we will focus on the AntiCheatService handling the logic 
            // when violation reports come in).
            
            // However, we CAN check if the request contains a "Suspicious" header or pattern
            // For example, simple bot detection
        }

        await _next(context);
    }
}
