using Microsoft.Identity.Client;
using System.Net;
using System.Text.Json;
using ToDo_Backend_CA_AplicationLayer.Exceptions;

namespace ToDo_Backend_FrameworksDrivers_API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogWarning("Execption handling middleware has been called.");
            _logger.LogError(ex, ex.Message);

            context.Response.ContentType = "application/json";
            
            context.Response.StatusCode = ex switch
            {
                InvalidUserCreationException => StatusCodes.Status400BadRequest,
                TaskIdValidationException => StatusCodes.Status404NotFound,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                InvalidOperationException => StatusCodes.Status400BadRequest,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                ValidateTokenException => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError,
            };

            var errorResponse = new
            {
                context.Response.StatusCode,
                ex.Message,
                Details = ex.InnerException?.Message
            };

            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}
