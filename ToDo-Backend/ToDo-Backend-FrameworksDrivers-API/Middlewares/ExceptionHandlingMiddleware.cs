using System.Net;
using System.Text.Json;
using ToDo_Backend_CA_AplicationLayer.Exceptions;

namespace ToDo_Backend_FrameworksDrivers_API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
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

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
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
                StatusCode = context.Response.StatusCode,
                Message = ex.Message,
                Details = ex.InnerException?.Message
            };

            return context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}
