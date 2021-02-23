using HomeWeather.Middleware;
using Microsoft.AspNetCore.Builder;

namespace HomeWeather.ServiceExtension
{
    public static class ExceptionExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
