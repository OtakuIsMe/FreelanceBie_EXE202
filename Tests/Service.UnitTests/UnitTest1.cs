using DotNetEnv;

namespace Service.UnitTests;

public class UnitTest1
{
	public UnitTest1() { }

	[Fact]
	public void TestEnvPath()
	{
		string envPath = Path.GetFullPath(Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory, "../../../../../BE/.env"));

		Console.WriteLine($"Environment Path: {envPath}");

		Env.Load(envPath);

		Assert.True(true);
	}
}
