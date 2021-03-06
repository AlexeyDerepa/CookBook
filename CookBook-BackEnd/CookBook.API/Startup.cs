using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CookBook.API.Filters;
using CookBook.API.Middlewares;
using CookBook.Application.Requests.GetRecipes;
using CookBook.DAL;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace CookBook.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<UnhandledExceptionMiddleware>();
            services.AddApplicationDataStores(Configuration);

            services.AddMvc(x =>
            {
                x.Filters.Add(typeof(ValidateModelAttribute));
            });
                        services.AddCors(config =>
            {
                config.AddPolicy("policy",
                builder => builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .SetIsOriginAllowed((host) => true /*hostSettings.GetSection("AllowedHosts").GetChildren().Any(x => x.Value == host)*/)
                );
            });
			
            services.AddMediatR(typeof(GetRecipeListCommand).Assembly);
            services.AddTransient<Mediator>();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "My API", 
                    Version = "v1",
                    Description = "API Documentation - Cook Book",
                    Contact = new OpenApiContact()
                    {
                        Name = "Global Logic", 
                        Url = new Uri("https://globallogic.com")
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<UnhandledExceptionMiddleware>();
			
			app.UseCors(builder =>
                builder.AllowAnyOrigin()
                       .AllowAnyHeader()
                       .AllowAnyMethod()
            );

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });       
            app.UseRouting();

         
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
