using HomeWeather.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace HomeWeather.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleware> logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            logger.LogError(exception.Message);
            context.Response.ContentType = "application/json";
            if (exception is AppException appException)
            {
                context.Response.StatusCode = appException.StatusCode;

                var responseContent = JsonConvert.SerializeObject(
                    new
                    {
                        appException.StatusCode,
                        appException.Message
                    });

                logger.LogError(responseContent);
                return context.Response.WriteAsync(responseContent);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return context.Response.WriteAsync(JsonConvert.SerializeObject(exception));
            }
        }
    }
}
