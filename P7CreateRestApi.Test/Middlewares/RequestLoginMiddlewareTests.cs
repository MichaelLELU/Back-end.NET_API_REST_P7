using Microsoft.AspNetCore.Http;
using P7CreateRestApi.Middlewares;
using P7CreateRestApi.Test.Helpers;
using Xunit;

namespace P7CreateRestApi.Tests.Middlewares
{
    public class RequestLoggingMiddlewareTests
    {
        [Fact]
        public async Task Middleware_Logs_Request_And_Response()
        {
            var logger = new ListLogger<RequestLoggingMiddleware>();

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Method = "GET";
            httpContext.Request.Path = "/test";

            RequestDelegate next = ctx =>
            {
                ctx.Response.StatusCode = 200;
                return Task.CompletedTask;
            };

            var middleware = new RequestLoggingMiddleware(next, logger);

            await middleware.InvokeAsync(httpContext);

            Assert.Contains(logger.Logs, log => log.Contains("/test"));
            Assert.Contains(logger.Logs, log => log.Contains("200"));
        }
    }
}
