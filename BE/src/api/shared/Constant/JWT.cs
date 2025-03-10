using DotNetEnv;

namespace BE.src.api.shared.Constant
{
	public static class JWT
	{
		static JWT()
		{
			Env.Load();
		}
		public static string SecretKey = Environment.GetEnvironmentVariable("JWT_SECURITY_KEY")
											?? throw new ApplicationException("JWT secret key not found in environment variables.");
		public static string Issuer = "FreelanceBie";
		public static string Audience = "User";
		public const int ACCESS_TOKEN_EXP = 10; // 10 minutes
  		public const int REFRESH_TOKEN_EXP = 7; // 7 days
	}
}
