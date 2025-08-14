using System.Net;
using AllPaintsEcomAPI.Services;

namespace AllPaintsEcomAPI.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                Exceptionlog.Logexception($"Error: {ex.Message}", $"File: {ex.StackTrace}");

                //await HandleExceptionAsync(httpContext, ex);
            }
        }
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var statusCode = (int)HttpStatusCode.InternalServerError;

            // Customize response for different exceptions
            if (exception is UnauthorizedAccessException)
            {
                statusCode = (int)HttpStatusCode.Unauthorized;
            }
            else if (exception is ArgumentException)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
            }

            context.Response.StatusCode = statusCode;
            Exceptionlog.Logexception(exception.Message, exception.StackTrace);

            var response = new { message = exception.Message };
            return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
        }

    }
}
