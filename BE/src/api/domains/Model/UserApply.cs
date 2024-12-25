using BE.src.api.domains.Model.Base;

namespace BE.src.api.domains.Model
{
	public class UserApply : BaseEntity
	{
		public required Guid UserId { get; set; }
		public User User { get; set; } = null!;

		public required Guid PostId { get; set; }
		public PostJob Post { get; set; } = null!;
	}
}

