namespace BE.src.api.domains.DTOs.Shot
{
	public class ShotAddData
	{
		public required string Title { get; set; }
		public required List<string> Specialties { get; set; }

		public required string Html { get; set; }
		public required List<FileShotAdd> Images { get; set; }
	}
	public class FileShotAdd
	{
		public required string Replace { get; set; }
		public required IFormFile File { get; set; }
	}
}
