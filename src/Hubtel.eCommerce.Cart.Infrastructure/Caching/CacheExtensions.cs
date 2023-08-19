using Hubtel.eCommerce.Cart.Core.Extensions.Caching;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hubtel.eCommerce.Cart.Infrastructure.Caching
{
    public static class CacheExtensions
    {
        public static IServiceCollection AddCaching(this IServiceCollection services)
        {
            services.AddEasyCaching(options =>
            {
                options.UseInMemory();
            });
            services.AddScoped<ICacheManager, CacheProvider>();
            return services;
        }
    }
}
