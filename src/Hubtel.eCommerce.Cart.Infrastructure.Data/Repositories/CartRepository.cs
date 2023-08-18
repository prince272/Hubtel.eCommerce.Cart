using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Hubtel.eCommerce.Cart.Core.Entities;
using Hubtel.eCommerce.Cart.Core.Repositories;
using Hubtel.eCommerce.Cart.Core.Extensions.Identity;
using Hubtel.eCommerce.Cart.Infrastructure.Data.Extensions;
using Hubtel.eCommerce.Cart.Core.Utilities;

namespace Hubtel.eCommerce.Cart.Infrastructure.Data.Repositories
{
    public class CartRepository : AppRepository<Core.Entities.Cart>, ICartRepository
    {
        public CartRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}