namespace HealthcareInventoryManagementSystem.Api.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Start measuring request execution time.
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Store the HTTP method and request path.
            var method = context.Request.Method;
            var path = context.Request.Path;

            try
            {
                // Continue processing the request.
                await _next(context);
            }
            finally
            {
                // Stop measuring execution time.
                stopwatch.Stop();

                // Get the HTTP response status code.
                var statusCode = context.Response.StatusCode;

                // Log request and response information.
                _logger.LogInformation(
                    "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds} ms",
                    method,
                    path,
                    statusCode,
                    stopwatch.ElapsedMilliseconds);
            }
        }
    }
}