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

        public static Type FindType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null)
                return type;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(typeName);
                if (type != null)
                    return type;
                foreach (var assemblyType in assembly.GetTypes())
                {
                    if (assemblyType.Name == typeName)
                        return assemblyType;
                }
            }
            return null;
        }

        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            if (!isInitializedConfiguration)
                InitializeConfigurations(services, configuration);

            services.AddScoped<IRepository<Sensor>, Repository<Sensor>>();
            services.AddScoped<IUnitOfWork<Sensor>, UnitOfWork<Sensor>>();
            services.AddScoped<IRepository<TempHistory>, Repository<TempHistory>>();
            services.AddScoped<IUnitOfWork<TempHistory>, UnitOfWork<TempHistory>>();

            services.AddScoped<ISensorService, SensorService>();
            services.AddScoped<ITempHistoryService, TempHistoryService>();
            services.AddSingleton<IWeatherForecastService, OpenWeatherForecastService>();

            Type serviceImplementer = FindType(configuration["ServiceImplementer"]);
            services.AddSingleton(serviceImplementer);
            services.AddSingleton(provider => (ISensorTempReader)provider.GetService(serviceImplementer));
            services.AddSingleton(provider => (IPhysSensorInfo)provider.GetService(serviceImplementer));
            services.AddSingleton(provider => (IHostedService)provider.GetService(serviceImplementer));            
        }

        public static void InitializeConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            if (!isInitializedConfiguration)
            {
                services.Configure<OpenWeatherSettings>(configuration);
                services.Configure<TempServiceSettings>(configuration);
                services.Configure<UARTSettings>(configuration);
                isInitializedConfiguration = true;
            }
        }
    }
}
