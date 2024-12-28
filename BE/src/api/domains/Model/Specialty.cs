using BE.src.api.domains.Model.Base;

namespace BE.src.api.domains.Model
{
	public class Specialty : BaseEntity
	{
		public required string Name { get; set; }

		public ICollection<PostJob> Posts { get; set; } = null!;
		public ICollection<Shot> Shots { get; set; } = null!;
	}
}
