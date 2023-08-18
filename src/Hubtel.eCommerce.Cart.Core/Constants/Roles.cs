using System;
using System.Collections.Generic;
using System.Text;

namespace Hubtel.eCommerce.Cart.Core.Constants
{
    public static class Roles
    {
        public static IEnumerable<string> All => new[] { Admin, Member };

        public static string Admin = nameof(Admin);

        public static string Member = nameof(Member);
    }
}
