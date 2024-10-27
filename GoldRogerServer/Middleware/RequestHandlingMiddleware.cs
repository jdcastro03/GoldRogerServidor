using System.Diagnostics.CodeAnalysis;

namespace GoldRogerServer.Middleware
{
    [ExcludeFromCodeCoverage]
    public class RequestHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestHandlingMiddleware> _logger;

        public RequestHandlingMiddleware(RequestDelegate next, ILogger<RequestHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            finally
            {
                string message = $"\"{context.Request?.Method} {context.Request?.Path.Value}\" {context.Response?.StatusCode} {context.Response?.ContentLength}";
                _logger.LogInformation(message);
            }
        }
    }
}

