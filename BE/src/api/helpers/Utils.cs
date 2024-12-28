using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

using Azure.Storage.Blobs;
using BE.src.api.domains.Enum;
using MyAzure = BE.src.api.shared.Constant.Azure;

namespace BE.src.api.helpers
{
	public static class Utils
	{
		private static BlobServiceClient blobServiceClient;
		static Utils()
		{
			blobServiceClient = new BlobServiceClient(MyAzure.ConnectionString);
		}
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

		public async static Task<string> GenerateAzureUrl(MediaTypeEnum type, IFormFile file, string objectName)
		{
			string containerName = (type == MediaTypeEnum.Image) ? MyAzure.containerImage : MyAzure.containerVideo;

			BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

			await containerClient.CreateIfNotExistsAsync();

			string objectNameType = objectName + ".png";

			BlobClient blobClient = containerClient.GetBlobClient(objectNameType);

			using (Stream fileStream = file.OpenReadStream())
			{
				await blobClient.UploadAsync(fileStream, overwrite: true);
			}

			return blobClient.Uri.ToString();
		}
	}
}
