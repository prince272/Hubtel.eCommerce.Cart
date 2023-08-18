using Hubtel.eCommerce.Cart.Core.Entities;
using Hubtel.eCommerce.Cart.Core.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Core.Repositories
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<Role> FindByNameAsync(string name);
    }
}
