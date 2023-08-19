using EasyCaching.Core;
using Hubtel.eCommerce.Cart.Core.Extensions.Caching;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Infrastructure.Caching
{
    public class CacheProvider : ICacheManager
    {
        private readonly IEasyCachingProvider _provider;
        private readonly int CacheMinutes = 30;

        public CacheProvider(IEasyCachingProvider provider)
        {
            _provider = provider;
        }

        public T Get<T>(string key, Func<T> acquire, int? cacheTime = null)
        {
            if (cacheTime <= 0)
                return acquire();

            return _provider.Get(key, acquire, TimeSpan.FromMinutes(cacheTime ?? CacheMinutes))
                .Value;
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> acquire, int? cacheTime = null)
        {
            if (cacheTime <= 0)
                return await acquire();

            var t = await _provider.GetAsync(key, acquire, TimeSpan.FromMinutes(cacheTime ?? CacheMinutes));
            return t.Value;
        }

        public async Task SetAsync(string key, object data, int cacheTime)
        {
            if (cacheTime <= 0)
                return;

            await _provider.SetAsync(key, data, TimeSpan.FromMinutes(cacheTime));
        }

        public Task<bool> IsSetAsync(string key)
        {
            return _provider.ExistsAsync(key);
        }

        public async Task<bool> PerformActionWithLockAsync(string key, TimeSpan expirationTime, Action action)
        {
            if (await _provider.ExistsAsync(key))
                return false;

            try
            {
                _provider.Set(key, key, expirationTime);

                action();

                return true;
            }
            finally
            {
                await RemoveAsync(key);
            }
        }

        public Task RemoveAsync(string key)
        {
            return _provider.RemoveAsync(key);
        }

        public Task RemoveByPrefixAsync(string prefix)
        {
            return _provider.RemoveByPrefixAsync(prefix);
        }

        public Task ClearAsync()
        {
            return _provider.FlushAsync();
        }

        public ValueTask<string> ComposeKeyAsync(string prefix, params object[] values)
        {
            string composedKey = prefix;

            if (values != null && values.Any())
            {
                foreach (var value in values)
                {
                    if (value == null)
                    {
                        composedKey += "-null";
                    }
                    else if (value is IEnumerable)
                    {
                        var enumValues = ((IEnumerable)value).Cast<object>().Select(x => x?.ToString() ?? "null");
                        composedKey += $"-{{{string.Join(",", enumValues)}}}";
                    }
                    else if (value != null && !IsSimpleType(value.GetType()))
                    {
                        var primValues = value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(x => x.CanRead && x.CanWrite).Select(x => x.GetValue(value) ?? "null");
                        composedKey += $"-{{{string.Join(",", primValues)}}}";
                    }
                    else
                    {
                        composedKey += $"-{{{value}}}";
                    }
                }
            }

            return new ValueTask<string>(composedKey);
        }

        #region IAsyncDisposable Support
        public virtual ValueTask DisposeAsync()
        {
            try
            {
                Dispose();
                return default;
            }
            catch (Exception exception)
            {
                return new ValueTask(Task.FromException(exception));
            }
        }
        #endregion

        #region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
            }

            disposed = true;
        }

        ~CacheProvider()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        private bool IsSimpleType(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                return IsSimpleType(typeInfo.GetGenericArguments()[0]);
            }
            return typeInfo.IsPrimitive
              || typeInfo.IsEnum
              || type.Equals(typeof(string))
              || type.Equals(typeof(decimal));
        }
    }
}
