using Newtonsoft.Json;
using Template.Application.Models.Results;

namespace Template.API.MIddlewares
{
    public static class ExceptionMiddlewareConfigure
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }

    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                var result = JsonConvert.SerializeObject(new InvalidResult<string>(ex.Message));
                await context.Response.WriteAsync(result);
            }
        }
    }
}
