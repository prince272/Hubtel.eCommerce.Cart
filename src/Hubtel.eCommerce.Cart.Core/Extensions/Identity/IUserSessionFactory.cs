using Hubtel.eCommerce.Cart.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Core.Extensions.Identity
{
    public interface IUserSessionFactory
    {
        Task<UserSessionInfo> GenerateAsync(User user);

        Task<bool> ValidateAccessTokenAsync(string accessToken);

        Task<bool> ValidateRefreshTokenAsync(string refreshToken);
    }
}
