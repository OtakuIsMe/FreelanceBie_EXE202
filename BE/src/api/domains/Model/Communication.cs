using BE.src.api.domains.Model.Base;

namespace BE.src.api.domains.Model
{
	public class Communication : BaseEntity
	{
		public required Guid ZeroId { get; set; }
		public User Zero { get; set; } = null!;

		public required Guid FirstId { get; set; }
		public User First { get; set; } = null!;

		public ICollection<Message> Messages { get; set; } = null!;
	}
}
