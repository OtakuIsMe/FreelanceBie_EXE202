using BE.src.api.domains.Model.Base;

namespace BE.src.api.domains.Model
{
	public class Follow : BaseEntity
	{
		public required Guid FollowingId { get; set; }
		public User Following { get; set; } = null!;

		public required Guid FollowedId { get; set; }
		public User Followed { get; set; } = null!;
	}
}
