using BE.src.api.domains.Model.Base;

namespace BE.src.api.domains.Model
{
	public class Notification : BaseEntity
	{
		public required string Message { get; set; }
		public required Guid UserId { get; set; }
		public User User { get; set; } = null!;
		public Guid? PostId { get; set; }
		public PostJob Post { get; set; } = null!;
		public int? CountUser { get; set; }
	}
}
