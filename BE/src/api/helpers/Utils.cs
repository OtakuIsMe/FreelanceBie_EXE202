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
