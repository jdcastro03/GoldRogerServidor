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

            // Agregar esta línea para ver el error completo incluyendo InnerException
            var fullErrorMessage = $"{exception.Message}\n{exception.StackTrace}\nInner: {exception.InnerException?.Message}";

            // Usar el mensaje completo en lugar del simple
            await WriteObject(APIResponse<bool>.Fail(fullErrorMessage), context);
        }

        private static async Task WriteObject(object obj, HttpContext context) => await context.Response.WriteAsync(JsonSerializer.Serialize(obj));
    }
}
