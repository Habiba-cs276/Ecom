using Ecom.Core.Entites;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.Infrastructure.Data;
using Ecom.Infrastructure.Data.Config;
using Ecom.Infrastructure.Repositries;
using Ecom.Infrastructure.Repositries.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure
{
    public static class InfrastructureRegisteration
    {
        public static IServiceCollection infrastructureConfiguration(this  IServiceCollection services,IConfiguration configuration)
        {
            //
            services.AddScoped(typeof(IGenericRepositry<,>), typeof(GenericRepositry<,>));
         
            
            //Unit Of Work
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

            //register email sender
            services.AddScoped<IEmailService, EmailService>();
            
            //Register Token SERVICE 
            services.AddScoped<IGenerateToken, GenerateToken>();    

            //
            services.AddDbContext<EcomDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("Ecom"));
            });
            services.AddScoped<IImageManagementService, ImageManagementService>();

            //add Redis Connection

            services.AddSingleton<IConnectionMultiplexer> ( i =>
            {
                var config = ConfigurationOptions.Parse(configuration.GetConnectionString("redis"));
                return ConnectionMultiplexer.Connect(config);   
            });
            services.AddScoped<ICustomerBasketRepositry, CustomerBasketRepositry>();

            // Auth Repo 
            services.AddScoped<IAuthRepositry, AuthRepositry>();


            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<EcomDbContext>().AddDefaultTokenProviders();

            // Add Auth 

            services.AddAuthentication(op =>
            {
                op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                op.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            }).AddCookie(c =>
            {
                c.Cookie.Name = "token";
                c.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;  
                };
            }).AddJwtBearer(op =>
            {
                op.RequireHttpsMetadata = false;
                op.SaveToken = true;
                op.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:Secret"])),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Token:Issure"],
                    ValidateAudience= false,
                    ClockSkew=TimeSpan.Zero,
                };
                op.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["token"];
                        return Task.CompletedTask;
                    }
                };
            });
            return services;
        }
    }
}
