using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.IServices
{
    public interface ICacheService
    {
        Task CachingAsync(string CacheKey, object Responce, TimeSpan ExpiredTime);
        Task<string?> GetCachedAsync(string CacheKey);
    }
}
