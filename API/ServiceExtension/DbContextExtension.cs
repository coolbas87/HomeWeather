using HomeWeather.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeWeather.ServiceExtension
{
    public static class DbContextExtension
    {
        public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DBConnection");

            services.AddDbContext<HomeWeatherContext>(options =>
                  options.UseSqlServer(connectionString, x => x.MigrationsAssembly("HomeWeather.Data")));
        }
    }
}
