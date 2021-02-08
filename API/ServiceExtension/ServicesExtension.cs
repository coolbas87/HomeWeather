using HomeWeather.Data.Entities;
using HomeWeather.Data.Infrastructure;
using HomeWeather.Data.Interfaces;
using HomeWeather.Domain.Configurations;
using HomeWeather.Domain.Services.Implementation;
using HomeWeather.Domain.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeWeather.ServiceExtension
{
    public static class ServicesExtension
    {
        private static bool isInitializedConfiguration = false;
        private static readonly List<Type> TempReaders = new List<Type>() { typeof(UART_TempReadingService), typeof(DummyTempReadingService), typeof(OpenWeatherTempReadingService) };

        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            if (!isInitializedConfiguration)
                InitializeConfigurations(services, configuration);

            services.AddScoped<IRepository<Sensors>, Repository<Sensors>>();
            services.AddScoped<IUnitOfWork<Sensors>, UnitOfWork<Sensors>>();
            services.AddScoped<IRepository<TempHistory>, Repository<TempHistory>>();
            services.AddScoped<IUnitOfWork<TempHistory>, UnitOfWork<TempHistory>>();

            Type serviceImplementer = TempReaders.FirstOrDefault(s => s.Name == configuration["ServiceImplementer"]);

            services.AddSingleton(serviceImplementer);
            services.AddSingleton(provider => (ITempReader)provider.GetService(serviceImplementer));
            services.AddSingleton(provider => (IHostedService)provider.GetService(serviceImplementer));            
        }

        public static void InitializeConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            if (!isInitializedConfiguration)
            {
                services.Configure<TempService>(configuration);
                services.Configure<OpenWeatherMap>(configuration);
            }
        }
    }
}
