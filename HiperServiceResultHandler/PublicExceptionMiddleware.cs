using HiperServiceResultHandler.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace HiperServiceResultHandler
{
    /// <summary>
    /// Exception middleware to use in the ASP.NET pipeline in order to encode exceptions
    /// in an instance of <see cref="ApiError"/>.
    /// </summary>
    public class PublicExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PublicExceptionMiddleware> _logger;
        private readonly Options _options;

        public class Options
        {
            public bool DevelopmentMode { get; set; } = false;
        }

        public PublicExceptionMiddleware(RequestDelegate next, ILogger<PublicExceptionMiddleware> logger, Options options = null)
        {
            _next = next;
            _logger = logger;
            _options = options ?? new Options();
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ServiceException exc)
            {
                _logger.LogError(exc, "Unhandled service exception, user message: {0}, message: {1}", exc.UserMessage, exc.Message);
                await HandleServiceExceptionAsync(httpContext, exc);
            }
            catch (Exception ex)
            {
                if (httpContext.RequestAborted.IsCancellationRequested)
                {
                    _logger.LogWarning(ex, "Request aborted: {0}", ex.Message);
                    return;
                }
                _logger.LogError(ex, "Unhandled error: {0}", ex.Message);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleServiceExceptionAsync(HttpContext context, ServiceException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(
                new ApiError(
                    (int)HttpStatusCode.InternalServerError,
                    exception.UserMessage ?? exception.Message,
                    exception.UserMessageCode, 
                    _options.DevelopmentMode ? exception : null
                ).ToString()
            );
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(
                new ApiError(
                    (int)HttpStatusCode.InternalServerError,
                    "Internal server error",
                    null,
                    _options.DevelopmentMode ? exception : null
                ).ToString()
            );
        }
    }

    public static class ConfigurePublicExceptionHandler
    {
        public static void ConfigurePublicServiceExceptionHandler(this IApplicationBuilder app, PublicExceptionMiddleware.Options options)
        {
            app.UseMiddleware<PublicExceptionMiddleware>(options);
        }
    }
}
