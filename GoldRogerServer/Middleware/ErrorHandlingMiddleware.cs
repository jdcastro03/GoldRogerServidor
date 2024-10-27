using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using GoldRogerServer.Utils;



namespace GoldRogerServer.Middleware
{
    [ExcludeFromCodeCoverage]
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            catch (Exception e)
            {
                _logger.LogError(Guid.NewGuid().ToString(), e, e.Message);
                await HandleException(context, e);
            }
        }

        private static async Task HandleException(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            // TODO: Change for a specific exception for validations
            if (!string.IsNullOrEmpty(exception.Message) && !string.IsNullOrWhiteSpace(exception.Message) && exception.Message.Contains("MSG"))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }

            await WriteObject(APIResponse<bool>.Fail(exception.Message), context);
        }

        private static async Task WriteObject(object obj, HttpContext context) => await context.Response.WriteAsync(JsonSerializer.Serialize(obj));
    }
}
