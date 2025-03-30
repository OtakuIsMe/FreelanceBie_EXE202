namespace BE.src.api.domains.DTOs.User
{
	public class DesignerCard
	{
		public List<ShotDesigner> Shots { get; set; } = null!;
		public List<string> Specialties { get; set; } = null!;
		public float? Price { get; set; }
		public string? Place { get; set; } = null!;
		public string Username { get; set; } = null!;
		public string UserImage { get; set; } = null!;
		public Guid UserId { get; set; }
	}

	public class ShotDesigner
	{
		public Guid Id { get; set; }
		public string Image { get; set; } = null!;
	}
}
