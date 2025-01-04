using BE.src.api.domains.Enum;

namespace BE.src.api.domains.DTOs.User
{
	public class SocialProfileDTO
	{
		public required TypeSocialEnum Type { get; set; }
		public required string Linked { get; set; }
	}
}
