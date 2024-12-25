using BE.src.api.domains.Model.Base;

namespace BE.src.api.domains.Model
{
	public class Like : BaseEntity
	{
		public required Guid ShotId { get; set; }
		public Shot Shot { get; set; } = null!;

		public required Guid UserId { get; set; }
		public User User { get; set; } = null!;
	}
}
