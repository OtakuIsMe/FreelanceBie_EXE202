using BE.src.api.domains.Model.Base;

namespace BE.src.api.domains.Model
{
	public class Message : BaseEntity
	{
		public required bool PersonIndex { get; set; }

		public required Guid CommunicationId { get; set; }
		public Communication Communication { get; set; } = null!;

		public required string Content { get; set; }

		public required int Index { get; set; }
	}
}
