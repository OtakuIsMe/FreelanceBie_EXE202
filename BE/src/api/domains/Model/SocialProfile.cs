using BE.src.api.domains.Enum;
using BE.src.api.domains.Model.Base;

namespace BE.src.api.domains.Model
{
	public class SocialProfile : BaseEntity
	{
		public required TypeSocialEnum Type { get; set; }
		public required string Linked { get; set; }

		public Guid UserId { get; set; }
		public User User { get; set; } = null!;
	}
}
