namespace BE.src.api.domains.DTOs.User
{
	public class DesignerCard
	{
		public List<string> Images { get; set; } = null!;
		public List<string> Specialties { get; set; } = null!;
		public int? Price { get; set; }
		public string Place { get; set; } = null!;
		public string Username { get; set; } = null!;
		public string UserImage { get; set; } = null!;
	}
}
