using BE.src.api.domains.Model.Base;

namespace BE.src.api.domains.Model
{
	public class Membership : BaseEntity
	{
		public required string Name { get; set; }
		public required float Price { get; set; }
		public required DateOnly ExpireTime { get; set; }
		public required string Description { get; set; } = null!;

		public ICollection<MembershipUser> MembershipUsers { get; set; } = null!;
	}
}
