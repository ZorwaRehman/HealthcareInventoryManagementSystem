using FluentValidation;
using System.Net;
using System.Text.Json;

namespace HealthcareInventoryManagementSystem.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Continue processing the HTTP request.
                await _next(context);
            }
            catch (ValidationException exception)
            {
                // Log validation errors.
                _logger.LogWarning(
                    exception,
                    "Request validation failed.");

                // Return HTTP 400 Bad Request.
                context.Response.StatusCode =
                    (int)HttpStatusCode.BadRequest;

                context.Response.ContentType = "application/json";

                var response = new
                {
                    statusCode = context.Response.StatusCode,
                    message = "Validation failed.",
                    errors = exception.Errors.Select(error => new
                    {
                        field = error.PropertyName,
                        message = error.ErrorMessage
                    })
                };

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(response));
            }
            catch (Exception exception)
            {
                // Log unexpected application errors.
                _logger.LogError(
                    exception,
                    "An unexpected error occurred while processing the request.");

                // Return HTTP 500 Internal Server Error.
                context.Response.StatusCode =
                    (int)HttpStatusCode.InternalServerError;

                context.Response.ContentType = "application/json";

                var response = new
                {
                    statusCode = context.Response.StatusCode,
                    message = "An unexpected error occurred. Please try again later."
                };

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(response));
            }
        }
    }
}
