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
											?? throw new InvalidOperationException("Connection string not found in environment variables.");
		public static string containerImage = "image";
		public static string containerVideo = "video";

		// --- RabbitMQ ---
		public static string RabbitMQHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST")
											?? throw new InvalidOperationException("RabbitMQ host not found in environment variables.");
		public static string RabbitMQPort = Environment.GetEnvironmentVariable("RABBITMQ_PORT")
											?? throw new InvalidOperationException("RabbitMQ host not found in environment variables.");									
		public static string RabbitMQUsername = Environment.GetEnvironmentVariable("RABBITMQ_USER")
											?? throw new InvalidOperationException("RabbitMQ username not found in environment variables.");

		public static string RabbitMQPassword = Environment.GetEnvironmentVariable("RABBITMQ_PASS")
											?? throw new InvalidOperationException("RabbitMQ password not found in environment variables.");											
	}
}
