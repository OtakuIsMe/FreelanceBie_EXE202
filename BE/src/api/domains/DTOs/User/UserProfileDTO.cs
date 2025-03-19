namespace BE.src.api.domains.DTOs.User
{
	public class UserProfileDTO
	{
		public string? Name { get; set; }
		public string? Slogan { get; set; }
		public string? Location { get; set; }
		public string Email { get; set; } = null!;
		public string? Language { get; set; }
		public string Username { get; set; } = null!;
		public DateTime JoinDate { get; set; }
		public string? Education { get; set; }
		public string Image { get; set; } = null!;
	}
}
