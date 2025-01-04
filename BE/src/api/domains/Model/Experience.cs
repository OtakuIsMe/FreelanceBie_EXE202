using BE.src.api.domains.Model.Base;

namespace BE.src.api.domains.Model
{
	public class Experience : BaseEntity
	{
		public required string JobTitle { get; set; }
		public required string Company { get; set; }
		public required DateOnly StartDate { get; set; }
		public required DateOnly EndDate { get; set; }
		public required string Description { get; set; }

		public Guid UserId { get; set; }
		public User User { get; set; } = null!;
	}
}
