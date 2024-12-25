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

		public required Guid UserId { get; set; }
		public User User { get; set; } = null!;

		public required Guid CompanyLogoId { get; set; }
		public required ImageVideo CompanyLogo { get; set; }

		public required Guid SpecialtyId { get; set; }
		public required Specialty Specialty { get; set; }

		public ICollection<Attachment> Attachments { get; set; } = null!;
		public ICollection<Save> Saves { get; set; } = null!;
		public ICollection<UserApply> UserApplies { get; set; } = null!;
	}
}
