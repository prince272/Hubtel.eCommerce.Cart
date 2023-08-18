﻿using Hubtel.eCommerce.Cart.Core.Entities;
using Hubtel.eCommerce.Cart.Core.Extensions.Identity;
using Hubtel.eCommerce.Cart.Core.Shared;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Core.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task CreateAsync(User user, string password);

        Task<User> FindByEmailAsync(string email);

        Task<User> FindByPhoneNumberAsync(string phoneNumber);

        Task AddToRoleAsync(User user, string role);

        Task AddToRolesAsync(User user, IEnumerable<string> roles);

        Task RemoveFromRoleAsync(User user, string role);

        Task RemoveFromRolesAsync(User user, IEnumerable<string> roles);

        Task<IEnumerable<string>> GetRolesAsync(User user);

        Task<bool> CheckPasswordAsync(User user, string password);

        Task<bool> HasPasswordAsync(User user);

        Task AddPasswordAsync(User user, string password);

        Task ChangePasswordAsync(User user, string currentPassword, string newPassword);

        Task RemovePasswordAsync(User user);

        Task<string> GenerateEmailTokenAsync(User user);

        Task VerifyEmailAsync(User user, string token);

        Task<string> GenerateChangeEmailTokenAsync(User user, string newEmail);

        Task ChangeEmailAsync(User user, string newEmail, string token);


        Task<string> GeneratePhoneNumberTokenAsync(User user);

        Task VerifyPhoneNumberTokenAsync(User user, string token);

        Task<string> GenerateChangePhoneNumberTokenAsync(User user, string newPhoneNumber);

        Task ChangePhoneNumberAsync(User user, string newPhoneNumber, string token);

        Task<string> GeneratePasswordResetTokenAsync(User user);

        Task ResetPasswordAsync(User user, string newPassword, string token);

        Task<UserSessionInfo> GenerateSessionAsync(User user);

        Task AddSessionAsync(User user, UserSessionInfo session);

        Task RemoveSessionAsync(User user, string token);

        Task AddLoginAsync(User user, UserLoginInfo login);

        Task RemoveLoginAsync(User user, string providerName, string providerKey);

        Task<User> FindByAccessTokenAsync(string accessToken);

        Task<User> FindByRefreshTokenAsync(string refreshToken);

        Task<bool> ValidateAccessTokenAsync(string accessToken);

        Task<bool> ValidateRefreshTokenAsync(string refreshToken);

        string GetDeviceId(ClaimsPrincipal principal);

        long? GetUserId(ClaimsPrincipal principal);

        string GetUserName(ClaimsPrincipal principal);

        string GetSecurityStamp(ClaimsPrincipal principal);

        Task GenerateUserNameAsync(User user);
    }
}
