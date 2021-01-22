using HomeWeather.Models;
using HomeWeather.Services;
using Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Services.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeWeather
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                            .AddJsonFile("Settings.json")
                            .AddConfiguration(configuration);
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }
        private List<Type> TempReaders = new List<ITempReader>() { typeof(TempReadingService), typeof(DummyTempReaderService) };

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<HWDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DBConnection")));
            services.AddControllersWithViews();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HomeWeather API", Version = "v1" });
            });

            ITempReader serviceImplementer = TempReaders.FirstOrDefault(s => s.Name == Configuration["ServiceImplementer"]);
            services.Configure<Settings>(Configuration);
            services.AddSingleton(serviceImplementer);
            services.AddSingleton(provider => (ITempReader)provider.GetService(serviceImplementer));
            services.AddSingleton(provider => (IHostedService)provider.GetService(serviceImplementer));
            services.AddSingleton<IDataBaseOperation, DatabaseOperations>();

            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "HomeWeather API V1");
                c.RoutePrefix = "docs";
            });
            app.UseCors(options => options.AllowAnyOrigin());

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
