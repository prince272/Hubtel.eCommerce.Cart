using Hubtel.eCommerce.Cart.Core.Shared;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hubtel.eCommerce.Cart.Core.Entities
{
    public class User : IdentityUser<long>, IEntity
    {
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }

    public class UserRole : IdentityUserRole<long>, IEntity
    {
        long IEntity.Id { get; set; }

        public virtual User User { get; set; }

        public virtual Role Role { get; set; }
    }
}
