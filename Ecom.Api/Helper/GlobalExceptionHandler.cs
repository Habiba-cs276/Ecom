using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace Ecom.Api.Helper
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, $"Unhandled Exception: {exception.Message}", exception.Message);
            
            int statusCode = (int)HttpStatusCode.InternalServerError;

            var response = new ResponseAPI<string>(statusCode, null, exception.Message);
 
            httpContext.Response.StatusCode = statusCode;   
            httpContext.Response.ContentType = "application/json";  
            await  httpContext.Response.WriteAsJsonAsync(response);
            return true;
        }
    }
}
