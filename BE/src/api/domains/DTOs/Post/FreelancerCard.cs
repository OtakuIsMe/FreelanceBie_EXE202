using BE.src.api.domains.Enum;

namespace BE.src.api.domains.DTOs.Post
{
	public class FreelancerCard
	{
		public required string Image { get; set; }
		public required string Username { get; set; }
		public required string Place { get; set; }
		public required float Price { get; set; }
		public required ApplyStatusEnum Status { get; set; }
		public required string Email { get; set; }
	}
}
