using BE.src.api.domains.Enum;
using BE.src.api.domains.Model.Base;

namespace BE.src.api.domains.Model
{
	public class Attachment : BaseEntity
	{
		public Guid PostId { get; set; }
		public PostJob Post { get; set; } = null!;
		public required string FileName { get; set; } = null!;
		public required FileTypeEnum FileType { get; set; }
		public byte[] FileContent { get; set; } = null!;
	}
}
