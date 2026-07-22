using Serilog.Context;

namespace OrderService.Middleware;

public class CorrelationIdMiddleware
{
    public const string HeaderName = "X-CorrelationId";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate requestDelegate)
    {
        _next = requestDelegate;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var existingValue)
        && !string.IsNullOrWhiteSpace(existingValue)
        ? existingValue.ToString()
        : Guid.NewGuid().ToString();


        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers[HeaderName] = correlationId;


        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}