using BE.src.api.domains.Enum;
using BE.src.api.domains.Model.Base;

namespace BE.src.api.domains.Model
{
	public class PostJob : BaseEntity
	{
		public required string Title { get; set; }
		public required string Description { get; set; }
		public required WorkTypeEnum WorkType { get; set; }
		public required string WorkLocation { get; set; }
		public required string CompanyName { get; set; }
		public required EmploymentTypeEnum EmploymentType { get; set; }
		public required int Experience { get; set; }
		public required string CompanyLink { get; set; }
		public required float Payment { get; set; }

		public Guid UserId { get; set; }
		public User User { get; set; } = null!;

		public Guid CompanyLogoId { get; set; }
		public ImageVideo CompanyLogo { get; set; } = null!;

		public required Guid SpecialtyId { get; set; }
		public Specialty Specialty { get; set; } = null!;

		public ICollection<Attachment> Attachments { get; set; } = null!;
		public ICollection<Save> Saves { get; set; } = null!;
		public ICollection<UserApply> UserApplies { get; set; } = null!;
	}
}
