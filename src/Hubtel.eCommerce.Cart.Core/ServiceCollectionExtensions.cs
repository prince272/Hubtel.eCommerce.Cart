using FluentValidation;
using Hubtel.eCommerce.Cart.Core.Entities;
using Hubtel.eCommerce.Cart.Core.Extensions.Identity;
using Hubtel.eCommerce.Cart.Core.Utilities;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hubtel.eCommerce.Cart.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddValidators(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Continue;
            ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
            ValidatorOptions.Global.DisplayNameResolver = (type, memberInfo, expression) =>
            {
                string RelovePropertyName()
                {
                    if (expression != null)
                    {
                        var chain = FluentValidation.Internal.PropertyChain.FromExpression(expression);
                        if (chain.Count > 0) return chain.ToString();
                    }

                    if (memberInfo != null)
                    {
                        return memberInfo.Name;
                    }

                    return null;
                }

                return RelovePropertyName()?.Humanize();
            };

            var validatorTypes = assemblies.SelectMany(_ => _.DefinedTypes).Select(_ => _.AsType()).Where(type => type.IsClass && !type.IsAbstract && !type.IsGenericType && type.IsCompatibleWith(typeof(IValidator<>)));

            foreach (var concreteType in validatorTypes)
            {
                var matchingInterfaceType = concreteType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>));

                if (matchingInterfaceType != null)
                {
                    services.AddScoped(concreteType);
                }
            }

            return services;
        }
    }
}