using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace sigma_api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            services.AddControllers(options =>
            {
                options.Filters.Add(new ExtractClaimsFilter());
            });

            // Swagger Configuration
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "sigma_api", Version = "v1" });

                // تعریف توکن سفارشی به عنوان یک هدر خاص
                var guidTokenScheme = new OpenApiSecurityScheme
                {
                    Name = "Token",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Description = "Enter your token here",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Token"
                    }
                };

                c.AddSecurityDefinition("Token", guidTokenScheme);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { guidTokenScheme, Array.Empty<string>() }
                });

                c.OperationFilter<AuthOperationFilter>();
            });

            // اضافه کردن اسکیم اتنتیکیشن سفارشی
            services.AddAuthentication("Authentication")
                .AddScheme<AuthenticationSchemeOptions, GuidAuthenticationHandler>("Authentication", null);

            services.AddAuthorization();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "sigma_api v1"));
            //}

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
