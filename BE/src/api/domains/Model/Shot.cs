using BE.src.api.domains.Model.Base;

namespace BE.src.api.domains.Model
{
	public class Shot : BaseEntity
	{
		public required string Title { get; set; }
		public required Guid UserId { get; set; }
		public User User { get; set; } = null!;

		public required string Html { get; set; }
		public required int View { get; set; }

		public ICollection<Comment> Comments { get; set; } = null!;
		public ICollection<ImageVideo> ImageVideos { get; set; } = null!;
		public ICollection<Save> Saves { get; set; } = null!;
		public ICollection<Like> Likes { get; set; } = null!;
		public ICollection<ViewAnalyst> ViewAnalysts { get; set; } = null!;
		public ICollection<Specialty> Specialties { get; set; } = null!;
	}
}
