using BE.src.api.repositories;
using StackExchange.Redis;

public class RedisPollingService : BackgroundService
{
	private readonly IConnectionMultiplexer _redis;
	private readonly IServiceScopeFactory _scopeFactory;
	private readonly ILogger<RedisPollingService> _logger;
	private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(30);

	public RedisPollingService(IConnectionMultiplexer redis, IServiceScopeFactory scopeFactory, ILogger<RedisPollingService> logger)
	{
		_redis = redis;
		_scopeFactory = scopeFactory;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var db = _redis.GetDatabase();

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				var keys = await GetKeysWithPrefixAsync("be_server:views_count:*");

				foreach (var key in keys)
				{
					string shotId = key.Replace("be_server:views_count:", "");
					int viewCount = (int)await db.StringGetAsync(key);

					if (viewCount > 0)
					{
						using (var scope = _scopeFactory.CreateScope())  // Tạo scope mới
						{
							var shotRepo = scope.ServiceProvider.GetRequiredService<IShotRepo>(); // Lấy service trong scope
							await shotRepo.AddViewShot(Guid.Parse(shotId), viewCount);
						}
						await db.KeyDeleteAsync(key);
						_logger.LogInformation($"Updated view count for shot {shotId}: {viewCount}");
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error in polling Redis keys: {ex.Message}");
			}

			await Task.Delay(_pollingInterval, stoppingToken); // Chờ 30s trước khi kiểm tra tiếp
		}
	}

	private async Task<List<string>> GetKeysWithPrefixAsync(string pattern)
	{
		var server = _redis.GetServer(_redis.GetEndPoints().First());
		return server.Keys(pattern: pattern).Select(k => (string)k).ToList();
	}
}
