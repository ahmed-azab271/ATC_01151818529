using Core.IServices;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase database;
        public CacheService(IConnectionMultiplexer Redis)
        {
            database = Redis.GetDatabase();
        }
        public async Task CachingAsync(string CacheKey, object Responce, TimeSpan ExpiredTime)
        {
            if (Responce is null) return;
            var Options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var SerilizedResponce = JsonSerializer.Serialize(Responce, Options);
            await database.StringSetAsync(CacheKey, SerilizedResponce , ExpiredTime);
        }

        public async Task<string?> GetCachedAsync(string CacheKey)
        {
            var Responce = await database.StringGetAsync(CacheKey);
            if (Responce.IsNullOrEmpty) return null;
            return Responce;
        }
    }
}
