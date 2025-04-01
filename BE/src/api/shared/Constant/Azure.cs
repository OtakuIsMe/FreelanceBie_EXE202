using DotNetEnv;
namespace BE.src.api.shared.Constant
{
	public static class Azure
	{
		static Azure()
		{
			Env.Load();
		}
		public static string ConnectionString = Environment.GetEnvironmentVariable("AZURE_KEY")
											?? throw new ApplicationException("Connection string not found in environment variables.");
		public static string containerImage = "image";
		public static string containerVideo = "video";

		// --- RabbitMQ ---
		// public static string RabbitMQHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST")
		// 									?? throw new ApplicationException("RabbitMQ host not found in environment variables.");
		// public static string RabbitMQPort = Environment.GetEnvironmentVariable("RABBITMQ_PORT")
		// 									?? throw new ApplicationException("RabbitMQ host not found in environment variables.");									
		// public static string RabbitMQUsername = Environment.GetEnvironmentVariable("RABBITMQ_USER")
		// 									?? throw new ApplicationException("RabbitMQ username not found in environment variables.");
		// public static string RabbitMQPassword = Environment.GetEnvironmentVariable("RABBITMQ_PASS")
		// 									?? throw new ApplicationException("RabbitMQ password not found in environment variables.");			

		// --- Redis ---
		public static string RedisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION")
											?? throw new ApplicationException("Redis connection string not found in environment variables.");		

		// --- MySQL Database(deploy on Azure) ---
		public static string MySQLConnectionString = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION")
											?? throw new ApplicationException("MySQL connection string not found in environment variables.");																								
	}
}
