﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheWorld.Services;
using Microsoft.Extensions.Configuration;
using TheWorld.Models;
using Newtonsoft.Json.Serialization;
using AutoMapper;
using TheWorld.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace TheWorld
{
    public class Startup
    {
        public IHostingEnvironment env { get; set; }
        public IConfigurationRoot config { get; set; }

        public Startup(IHostingEnvironment env)
        {
            this.env = env;
            var builder = new ConfigurationBuilder()
                .SetBasePath(this.env.ContentRootPath)
                .AddJsonFile("config.json")
                // this will overwrite the config.json variables and use the environment variables of the production machine 
                .AddEnvironmentVariables();

            this.config = builder.Build();

        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(this.config);
            if(env.IsEnvironment("Development") || env.IsEnvironment("Testing"))
            {
                // Go to the world project right click properties to set the environment            
                services.AddScoped<IMailService, DebugMailService>();
            }
            else
            {
                // Implement production
            }
            services.AddDbContext<WorldContext>();

            services.AddScoped<IWorldRepository, WorldRepository>();

            services.AddTransient<GeoCoordsService>();

            services.AddTransient<WorldContextSeedData>();

            services.AddLogging();

            services.AddMvc(config => {

                if (this.env.IsProduction())
                {
                    config.Filters.Add(new RequireHttpsAttribute());
                }
                

            }).AddJsonOptions(config => config.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

            services.AddIdentity<WorldUser, IdentityRole>(config =>
           {
               config.User.RequireUniqueEmail = true;
               config.Password.RequiredLength = 8;
               config.Cookies.ApplicationCookie.LoginPath = "/Auth/Login";
               config.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents()
               {
                   OnRedirectToLogin = async ctx =>
                   {
                       if (ctx.Request.Path.StartsWithSegments("/api") && 
                           ctx.Response.StatusCode == 200)
                       {
                           ctx.Response.StatusCode = 401;
                       }
                       else
                       {
                           ctx.Response.Redirect(ctx.RedirectUri);
                       }
                       await Task.Yield();
                   }
               };
           })
            .AddEntityFrameworkStores<WorldContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
            IHostingEnvironment env, 
            WorldContextSeedData seeder,
            ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddConsole();
            Mapper.Initialize(config =>
           {
               config.CreateMap<TripViewModel, Trip>().ReverseMap();
               config.CreateMap<StopViewModel, Stop>().ReverseMap();

           });



            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                loggerFactory.AddDebug(LogLevel.Information);
            }
            else
            {
                loggerFactory.AddDebug(LogLevel.Error);
            }

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});

            //app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            app.UseIdentity();

            app.UseMvc(config =>
            {
                config.MapRoute(
                    name: "Default",
                    template: "{Controller}/{action}/{id?}",
                    defaults: new { controller = "App", action = "Index" }
                    );

            });

            seeder.EnsureSeedData().Wait();
          
        }
    }
}