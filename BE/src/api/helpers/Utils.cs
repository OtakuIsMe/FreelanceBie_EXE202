using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace BE.src.api.helpers
{
	public static class Utils
	{
		public static T ToEnum<T>(this string value)
		{
			return (T)System.Enum.Parse(typeof(T), value, true);
		}

		public static string HashObject<T>(T obj)
        {
            string json = JsonConvert.SerializeObject(obj);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                byte[] hashBytes = sha256.ComputeHash(bytes);

                StringBuilder hashString = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    hashString.Append(b.ToString("x2"));
                }

                return hashString.ToString();
            }
        }
	}
}
