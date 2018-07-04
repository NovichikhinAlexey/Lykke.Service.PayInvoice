using System;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Services;
using StackExchange.Redis;

namespace Lykke.Service.PayInvoice.Services
{
    public class RedisLocksService : IDistributedLocksService
    {
        private readonly string _keyPattern;
        private readonly IDatabase _database;

        public RedisLocksService(
            string keyPattern,
            IConnectionMultiplexer connectionMultiplexer)
        {
            _keyPattern = keyPattern;
            _database = connectionMultiplexer.GetDatabase();
        }

        public Task<bool> TryAcquireLockAsync(string key, string token, DateTime expiration)
        {
            TimeSpan expiresIn = expiration - DateTime.UtcNow;

            return _database.LockTakeAsync(GetCacheKey(key), token, expiresIn);
        }

        public Task<bool> ReleaseLockAsync(string key, string token)
        {
            return _database.LockReleaseAsync(GetCacheKey(key), token);
        }

        private string GetCacheKey(string key)
        {
            return string.Format(_keyPattern, key);
        }
    }
}
