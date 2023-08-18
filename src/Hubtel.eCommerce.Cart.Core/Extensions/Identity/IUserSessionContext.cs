﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Core.Extensions.Identity
{
    public interface IUserSessionContext
    {
        string DeviceId { get; }

        ClaimsPrincipal User { get; }
    }
}
