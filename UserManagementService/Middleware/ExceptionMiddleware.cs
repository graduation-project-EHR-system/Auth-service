using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using UserManagementService.Error;

namespace UserManagementService.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex , ex.Message);
                
                 httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                 httpContext.Response.ContentType = "application/json";

                var response = _env.IsDevelopment()  ?
                    new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString())
                    :
                    new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(response , options);

                await httpContext.Response.WriteAsync(json);
            }
        }
    }

    public static class MiddleWareExtention
    {
        public static IApplicationBuilder UseExceptionMiddleWare(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
