using BE.src.api.domains.Model.Base;

namespace BE.src.api.domains.Model
{
	public class Attachment : BaseEntity
	{
		public required Guid PostId { get; set; }
		public PostJob Post { get; set; } = null!;
		public required string FileName { get; set; } = null!;
		public byte[]? FileContent { get; set; }
	}
}
