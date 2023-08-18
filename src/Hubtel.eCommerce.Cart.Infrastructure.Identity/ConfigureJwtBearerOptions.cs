﻿using Hubtel.eCommerce.Cart.Core.Extensions.Identity;
using Hubtel.eCommerce.Cart.Core.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Infrastructure.Identity
{
    public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly IOptions<UserSessionOptions> _userSessionOptions;

        public ConfigureJwtBearerOptions(IOptions<UserSessionOptions> userSessionOptions)
        {
            _userSessionOptions = userSessionOptions ?? throw new ArgumentNullException(nameof(userSessionOptions));
        }

        public void Configure(JwtBearerOptions options)
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters ??= new TokenValidationParameters();
            options.TokenValidationParameters.ValidateIssuer = true;
            options.TokenValidationParameters.ValidIssuers = _userSessionOptions.Value.GetIssuers();
            options.TokenValidationParameters.ValidateAudience = true;
            options.TokenValidationParameters.ValidAudiences = _userSessionOptions.Value.GetAudiences();
            options.TokenValidationParameters.ValidateLifetime = true;
            options.TokenValidationParameters.ValidateIssuerSigningKey = true;
            options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_userSessionOptions.Value.Secret));

            options.TokenValidationParameters.ClockSkew = TimeSpan.Zero;

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                    logger.LogError($"Authentication failed {context.Exception}");
                    return Task.CompletedTask;
                },
                OnTokenValidated = async context =>
                {
                    var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
                    var userSessionContext = context.HttpContext.RequestServices.GetRequiredService<IUserSessionContext>();

                    var claimsPrincipal = context.Principal;

                    if (claimsPrincipal?.Claims == null || !claimsPrincipal.Claims.Any())
                    {
                        context.Fail("This is not our issued token. It has no claims.");
                        return;
                    }

                    var deviceId = userRepository.GetDeviceId(claimsPrincipal);
                    if (deviceId == null || !string.Equals(deviceId, userSessionContext.DeviceId, StringComparison.Ordinal))
                    {
                        context.Fail("Detected usage of an old token from an unknown device! Please login again!");
                        return;
                    }

                    var userId = userRepository.GetUserId(claimsPrincipal);
                    var user = await userRepository.FindByIdAsync(userId ?? -1);

                    var securityStamp = userRepository.GetSecurityStamp(claimsPrincipal);

                    if (user == null || !string.Equals(user.SecurityStamp, securityStamp, StringComparison.Ordinal) || !user.Active)
                    {
                        // user has changed his/her password/roles/active
                        context.Fail("This token is expired. Please login again.");
                        return;
                    }

                    if (!(context.SecurityToken is JwtSecurityToken accessToken) || string.IsNullOrWhiteSpace(accessToken.RawData) ||
                                      !await userRepository.ValidateAccessTokenAsync(accessToken.RawData))
                    {
                        context.Fail("This token is not in our database.");
                        return;
                    }

                    // Updates the ActiveAt of a user only if more than 2 minutes have passed since the last update.
                    var current = DateTimeOffset.UtcNow;
                    if (current - user.ActiveAt >= TimeSpan.FromMinutes(2))
                    {
                        user.ActiveAt = current;
                        await userRepository.UpdateAsync(user);
                    }
                },
                OnMessageReceived = context => Task.CompletedTask,
                OnChallenge = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                    logger.LogError($"OnChallenge error {context.Error}, {context.ErrorDescription}");
                    return Task.CompletedTask;
                },
            };
        }

        public void Configure(string name, JwtBearerOptions options)
        {
            if (name == JwtBearerDefaults.AuthenticationScheme) Configure(options);
        }
    }
}