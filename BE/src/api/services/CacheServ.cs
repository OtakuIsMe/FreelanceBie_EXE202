using System.Text.Json;
using StackExchange.Redis;

namespace BE.src.api.services
{
    public interface ICacheService
    {
        Task<T?> Get<T>(string key);
        Task Set<T>(string key, T value);
        Task Set<T>(string key, T value, TimeSpan expiration);
        Task<bool> Update<T>(string key, T value);
        Task Remove(string key);
        Task<bool> Exists(string key);
        Task ClearWithPattern(string prefix);
    }
	public class CacheServ : ICacheService
	{
        private readonly IDatabase _database;
        private readonly string _redisKey = "be_server";
        public CacheServ(IConnectionMultiplexer connectionMultiplexer)
        {
            _database = connectionMultiplexer.GetDatabase();
        }

		public Task ClearWithPattern(string prefix)
		{
			var endpoints = _database.Multiplexer.GetEndPoints();
            var server = _database.Multiplexer.GetServer(endpoints.First());
            var keys = server.Keys(pattern: $"{_redisKey}:{prefix}*").ToArray();
            return _database.KeyDeleteAsync(keys);
		}

		public Task<bool> Exists(string key)
		{
			return _database.KeyExistsAsync($"{_redisKey}:{key}");
		}

		public async Task<T?> Get<T>(string key)
		{
			var value = await _database.StringGetAsync($"{_redisKey}:{key}");
            if (value.IsNullOrEmpty)
                return default(T);

            return JsonSerializer.Deserialize<T>(value);
		}

		public Task Remove(string key)
		{
			return _database.KeyDeleteAsync($"{_redisKey}:{key}");
		}

		public Task Set<T>(string key, T value)
		{
			return _database.StringSetAsync($"{_redisKey}:{key}", JsonSerializer.Serialize(value), TimeSpan.FromMinutes(30));
		}

		public Task Set<T>(string key, T value, TimeSpan expiration)
		{
			return _database.StringSetAsync($"{_redisKey}:{key}", JsonSerializer.Serialize(value), expiration);
		}

		public Task<bool> Update<T>(string key, T value)
		{
			TimeSpan? timeSpan = _database.KeyTimeToLive($"{_redisKey}:{key}");
            return _database.StringSetAsync($"{_redisKey}:{key}", JsonSerializer.Serialize(value), timeSpan);
		}
	}
}