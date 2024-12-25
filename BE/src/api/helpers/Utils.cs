namespace BE.src.api.helpers
{
	public static class Utils
	{
		public static T ToEnum<T>(this string value)
		{
			return (T)System.Enum.Parse(typeof(T), value, true);
		}
	}
}
