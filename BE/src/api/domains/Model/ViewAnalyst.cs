using BE.src.api.domains.Model.Base;

namespace BE.src.api.domains.Model
{
	public class ViewAnalyst : BaseEntity
	{
		public required DateOnly Date { get; set; }

		public required int View { get; set; }

		public required Guid ShotId { get; set; }
		public Shot Shot { get; set; } = null!;
	}
}
