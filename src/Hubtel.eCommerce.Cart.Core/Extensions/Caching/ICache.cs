using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Core.Extensions.Caching
{
    public interface ICacheManager : IDisposable, IAsyncDisposable
    {
        Task ClearAsync();
        T Get<T>(string key, Func<T> acquire, int? cacheTime = null);
        Task<T> GetAsync<T>(string key, Func<Task<T>> acquire, int? cacheTime = null);
        Task<bool> IsSetAsync(string key);
        Task<bool> PerformActionWithLockAsync(string key, TimeSpan expirationTime, Action action);
        Task RemoveAsync(string key);
        Task RemoveByPrefixAsync(string prefix);
        Task SetAsync(string key, object data, int cacheTime);
        ValueTask<string> ComposeKeyAsync(string prefix, params object[] values);
    }
}
