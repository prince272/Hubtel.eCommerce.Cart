
using Hubtel.eCommerce.Cart.Core.Entities;
using Hubtel.eCommerce.Cart.Core.Extensions.Identity;
using Hubtel.eCommerce.Cart.Core.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Hubtel.eCommerce.Cart.Infrastructure.Identity
{
    public static class ServiceCollectionExtensions
    {
        public static AuthenticationBuilder AddBearer(this AuthenticationBuilder builder, Action<UserSessionOptions> options)
        {
            builder.Services.AddOptions<UserSessionOptions>().Configure<IHttpContextAccessor>((optionsInstance, httpContextAccessor) => {
                ConfigureBearer(() => options(optionsInstance), optionsInstance, httpContextAccessor);
            });
            builder.AddBearer();
            return builder;
        }

        public static AuthenticationBuilder AddBearer(this AuthenticationBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddOptions<UserSessionOptions>().Configure<IHttpContextAccessor>((options, httpContextAccessor) => {
                ConfigureBearer(() => configuration.Bind(options), options, httpContextAccessor);
            });
            builder.AddBearer();
            return builder;
        }

        private static void ConfigureBearer(Action configure, UserSessionOptions options, IHttpContextAccessor httpContextAccessor)
        {
            configure();

            var context = httpContextAccessor?.HttpContext;
            var serverUrl = context != null ? string.Concat(context.Request.Scheme, "://", context.Request.Host.ToUriComponent()) : string.Empty;

            var separator = UserSessionOptions.ValueSeparator;

            options.Secret = !string.IsNullOrEmpty(options.Secret) ? options.Secret : AlgorithmHelper.SecretKey;

            options.Issuer = string.Join(separator, (options.Issuer ?? string.Empty).Split(separator).Append(serverUrl).Distinct().SkipWhile(string.IsNullOrEmpty).ToArray());
            options.Audience = string.Join(separator, (options.Issuer ?? string.Empty).Split(separator).Append(serverUrl).Distinct().SkipWhile(string.IsNullOrEmpty).ToArray());

            options.AccessTokenExpiresIn = options.AccessTokenExpiresIn != TimeSpan.Zero ? options.AccessTokenExpiresIn : TimeSpan.FromDays(1);
            options.RefreshTokenExpiresIn = options.AccessTokenExpiresIn != TimeSpan.Zero ? options.AccessTokenExpiresIn : TimeSpan.FromDays(90);
        }

        public static AuthenticationBuilder AddBearer(this AuthenticationBuilder builder)
        {
            builder.Services.ConfigureOptions<ConfigureJwtBearerOptions>();

            builder.Services.AddScoped<IUserClaimsPrincipalFactory<User>, UserClaimsPrincipalFactory>();
            builder.Services.AddScoped<IUserSessionFactory, UserSessionFactory>();
            builder.Services.AddScoped<IUserSessionStore, UserSessionStore>();
            builder.Services.AddScoped<IUserContext, UserSessionContext>();


            return builder.AddJwtBearer();
        }
    }
}