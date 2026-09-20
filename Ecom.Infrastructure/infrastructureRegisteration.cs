using Ecom.Core.Interfaces;
using Ecom.Infrastructure.Data;
using Ecom.Infrastructure.Data.Config;
using Ecom.Infrastructure.Repositries;
using Ecom.Infrastructure.Repositries.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
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
            services.AddScoped(typeof(IGenericRepositry<,>), typeof(GenericRepositry<,>));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
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
          
            return services;
        }
    }
}
