using HomeWeather.Controllers;
using HomeWeather.Models;
using HomeWeather.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
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
        private List<Type> TempReaders = new List<Type>() { typeof(TempReadingService), typeof(DummyTempReaderService) };

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<Settings>(Configuration);
            services.AddDbContext<HWDbContext>(options => options.UseSqlServer(Configuration["TestLocal"]));
            services.AddControllersWithViews();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HomeWeather API", Version = "v1" });
            });

            Type serviceImplementer = TempReaders.FirstOrDefault(s => s.Name == Configuration["ServiceImplementer"]);
            services.AddSingleton(serviceImplementer);
            services.AddSingleton<ITempReader>(provider => (ITempReader)provider.GetService(serviceImplementer));
            services.AddSingleton<IHostedService>(provider => (IHostedService)provider.GetService(serviceImplementer));

            services.AddCors();

            // In production, the React files will be served from this directory
            //services.AddSpaStaticFiles(configuration =>
            //{
                //configuration.RootPath = "UI/build";
            //});
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
                c.RoutePrefix = string.Empty;
            });
            app.UseCors(options => options.AllowAnyOrigin());
            //app.UseStaticFiles();
            //app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            //app.UseSpa(spa =>
            //{
            //    spa.Options.SourcePath = "ClientApp";

            //    if (env.IsDevelopment())
            //    {
            //        spa.UseReactDevelopmentServer(npmScript: "start");
            //    }
            //});
        }
    }
}
