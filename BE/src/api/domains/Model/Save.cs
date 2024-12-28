using BE.src.api.domains.Model.Base;

namespace BE.src.api.domains.Model
{
	public class Save : BaseEntity
	{
		public required Guid UserId { get; set; }
		public User User { get; set; } = null!;

		public Guid? PostId { get; set; }
		public PostJob Post { get; set; } = null!;

		public Guid? ShotId { get; set; }
		public Shot Shot { get; set; } = null!;
	}
}
