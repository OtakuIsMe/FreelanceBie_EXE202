namespace BE.src.api.domains.DTOs.Shot
{
	public class ShotAddData
	{
		public required Guid UserId { get; set; }
		public required Guid SpecialtyId { get; set; }

		public required string Html { get; set; }
		public required string Css { get; set; }
		public required List<IFormFile> Images { get; set; }
	}
}
