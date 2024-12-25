using BE.src.api.domains.Enum;
using BE.src.api.domains.Model.Base;

namespace BE.src.api.domains.Model
{
	public class ImageVideo : BaseEntity
	{
		public required MediaTypeEnum Type { get; set; }

		public Guid? ShotId { get; set; }
		public Shot Shot { get; set; } = null!;
		public Guid? UserId { get; set; }
		public User User { get; set; } = null!;

		public required string Url { get; set; }

		public PostJob? Post { get; set; } = null;
	}
}
