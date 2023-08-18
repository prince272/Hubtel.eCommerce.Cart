using Bogus;
using Hubtel.eCommerce.Cart.Core.Constants;
using Hubtel.eCommerce.Cart.Core.Entities;
using Hubtel.eCommerce.Cart.Core.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Infrastructure.Data
{
    // How to seed data in .NET Core 6 with Entity Framework?
    // source: https://stackoverflow.com/questions/70581816/how-to-seed-data-in-net-core-6-with-entity-framework
    public class AppDbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            // Get a logger
            var logger = services.GetRequiredService<ILogger<AppDbInitializer>>();


            var dbContext = services.GetRequiredService<AppDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            logger.LogInformation("Start seeding the database.");

            var roleRepository = services.GetRequiredService<IRoleRepository>();

            if (!(await roleRepository.AnyAsync()))
            {
                foreach (var roleName in Roles.All)
                {
                    await roleRepository.CreateAsync(new Role(roleName));
                }
            }

            var itemRepository = services.GetRequiredService<IItemRepository>();

            if (!(await itemRepository.AnyAsync()))
            {
                var itemsFaker = new Faker<Item>();
                itemsFaker.RuleFor(i => i.Name, i => i.Commerce.ProductName())
                          .RuleFor(i => i.Price, i => decimal.Parse(i.Commerce.Price()))
                          .RuleFor(i => i.CreatedAt, i => i.Date.RecentOffset(5))
                          .RuleFor(i => i.UpdatedAt, i => i.Date.RecentOffset(2));

                var items = itemsFaker.GenerateBetween(100, 200);

                foreach (var item in items)
                {
                    await itemRepository.CreateAsync(item);
                }
            }

            logger.LogInformation("Finished seeding the database.");
        }
    }
}
