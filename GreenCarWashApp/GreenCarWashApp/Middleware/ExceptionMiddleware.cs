namespace GreenCarWashApp.Middleware
{
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Net;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext); 
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var (statusCode, message) = GetErrorResponse(exception);
            response.StatusCode = statusCode;

            var errorResponse = new
            {
                StatusCode = statusCode,
                Message = message,
                Details = exception.Message,
                Timestamp = DateTime.UtcNow
            };

            var errorJson = JsonSerializer.Serialize(errorResponse);
            return context.Response.WriteAsync(errorJson);
        }

        private static (int statusCode, string message) GetErrorResponse(Exception exception)
        {
            return exception switch
            {
                UnauthorizedAccessException => (401, "Unauthorized access"),
                ArgumentException => (400, "Invalid request data"),
                KeyNotFoundException => (404, "Resource not found"),
                InvalidOperationException => (400, "Invalid operation"),
                TimeoutException => (408, "Request timeout"),
                Microsoft.EntityFrameworkCore.DbUpdateException => (409, "Database conflict"),
                _ => (500, "Internal server error")
            };
        }
    }

}
