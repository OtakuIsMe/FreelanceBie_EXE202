using BE.src.api.domains.Enum;

namespace BE.src.api.domains.DTOs.User
{
	public class UserAddData
	{
		public required string Name { get; set; }
		public required string Username { get; set; }
		public required string Email { get; set; }
		public required string Password { get; set; }
		public required RoleEnum Role { get; set; }
		public string? Phone { get; set; }
		public string? City { get; set; }
		public string? Education { get; set; }
		public string? Description { get; set; }
		public DateOnly? DOB { get; set; }
		public IFormFile? Avatar { get; set; }
		public IFormFile? Background { get; set; }
		public List<SocialProfileDTO>? SocialProfiles { get; set; }
	}
}
