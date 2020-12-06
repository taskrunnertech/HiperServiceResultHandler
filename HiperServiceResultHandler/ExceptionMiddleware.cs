﻿using HiperServiceResultHandler.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace HiperServiceResultHandler
{
    public class ExceptionMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            //TODO: we should handle custom exceptions here
            catch (ServiceExceptions exc)
            {
                _logger.LogError(exc.ErrorCode, exc, $"User Message:{exc.UserMessage} | Message: {exc.Message}");
                await HandleExceptionAsync(httpContext, exc);
            }
        }
        private Task HandleExceptionAsync(HttpContext context, ServiceExceptions exc)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            return context.Response.WriteAsync(
                new ApiResultMessage
                {
                    Data = null,
                    UserMessage = exc.UserMessage,
                    UserMessageCode = exc.UserMessageCode,
                    ErrorCode = exc.ErrorCode,
                    Message = exc.Message,
                    Exception = exc.ToString()
                }.ToString()
                );
        }
    }

    public static class ConfigureExceptionHandler
    {
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}