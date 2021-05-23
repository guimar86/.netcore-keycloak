using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace keytest
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "keytest", Version = "v1" });
            });

            services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;


        }).AddJwtBearer(o =>
         {
             o.Authority = Configuration["Jwt:Authority"];
             o.Audience = Configuration["Jwt:Audience"];
             o.RequireHttpsMetadata = false;
             o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
             {
                 ValidAudiences = new string[] { "master-realm", "account", o.Audience }
             };
             o.Events = new JwtBearerEvents()
             {
                 OnAuthenticationFailed = c =>
                {
                    c.NoResult();

                    c.Response.StatusCode = 500;
                    c.Response.ContentType = "text/plain";
                    if (Environment.IsDevelopment())
                    {
                        return c.Response.WriteAsync(c.Exception.ToString());
                    }
                    return c.Response.WriteAsync("An error occured processing your authentication.");
                }
             };
         });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "keytest v1"));
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
