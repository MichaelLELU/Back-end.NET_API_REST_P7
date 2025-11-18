using System.Diagnostics;

namespace P7CreateRestApi.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var user = context.User.Identity?.Name ?? "Anonyme";

            _logger.LogInformation("➡️ {Method} {Path} appelé par {User}",
                context.Request.Method, context.Request.Path, user);

            await _next(context);

            stopwatch.Stop();

            _logger.LogInformation("⬅️ {Method} {Path} terminé avec statut {StatusCode} ({Elapsed} ms)",
                context.Request.Method, context.Request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
        }
    }


    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
