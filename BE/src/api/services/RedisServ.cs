using StackExchange.Redis;
using Newtonsoft.Json;

namespace BE.src.api.services
{
	public interface IRedisServ
	{
		Task<int> GetDataByKey(string key);
		Task SetDataByKey(string key, int value);
		Task IncreBy(string key, int increValue);
		Task DecreBy(string key, int DecreValue);
	}

	public class RedisServ : IRedisServ
	{
		private readonly IConnectionMultiplexer _redis;

		public RedisServ(IConnectionMultiplexer redis)
		{
			_redis = redis;
		}

		public async Task<int> GetDataByKey(string key)
		{
			try
			{
				var db = _redis.GetDatabase();
				var value = await db.StringGetAsync(key);
				return (int)value;
			}
			catch (System.Exception)
			{
				throw;
			}
		}
		public async Task SetDataByKey(string key, int value)
		{
			try
			{
				var db = _redis.GetDatabase();
				await db.StringSetAsync(key, value);
			}
			catch (System.Exception)
			{
				throw;
			}
		}
		public async Task IncreBy(string key, int increValue)
		{
			try
			{
				var db = _redis.GetDatabase();
				await db.StringIncrementAsync(key, increValue);
			}
			catch (System.Exception)
			{
				throw;
			}
		}
		public async Task DecreBy(string key, int DecreValue)
		{
			try
			{
				var db = _redis.GetDatabase();
				await db.StringDecrementAsync(key, DecreValue);
			}
			catch (System.Exception)
			{
				throw;
			}
		}
	}
}
