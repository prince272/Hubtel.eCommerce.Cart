using Hubtel.eCommerce.Cart.Core.Entities;
using Hubtel.eCommerce.Cart.Core.Repositories;
using Hubtel.eCommerce.Cart.Infrastructure.Data.Extensions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Infrastructure.Data.Repositories
{
    public class RoleRepository : AppRepository<Role>, IRoleRepository
    {
        private readonly RoleManager<Role> _roleManager;

        public RoleRepository(RoleManager<Role> roleManager, AppDbContext dbContext) : base(dbContext)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        public override async Task CreateAsync(Role role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));

            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded) throw new InvalidOperationException(result.Errors.GetMessage());
        }

        public override async Task UpdateAsync(Role role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));

            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded) throw new InvalidOperationException(result.Errors.GetMessage());
        }

        public override async Task DeleteAsync(Role role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));

            var result = await _roleManager.DeleteAsync(role);

            if (!result.Succeeded) throw new InvalidOperationException(result.Errors.GetMessage());
        }

        public Task<Role> FindByNameAsync(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return _roleManager.FindByNameAsync(name);
        }
    }
}
