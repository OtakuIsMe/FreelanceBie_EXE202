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
	}
}
