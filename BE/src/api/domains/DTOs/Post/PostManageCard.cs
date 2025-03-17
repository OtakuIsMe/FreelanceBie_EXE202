using BE.src.api.domains.Enum;

namespace BE.src.api.domains.DTOs.Post
{
	public class PostManageCard
	{
		public required string Title { get; set; } = null!;
		public required bool Status { get; set; }
		public required WorkTypeEnum WorkType { get; set; }
		public required string WorkLocation { get; set; } = null!;
		public required EmploymentTypeEnum EmploymentType { get; set; }
		public required DateTime CreateAt { get; set; }
		public required DateTime CloseAt { get; set; }
		public required int NumberApplied { get; set; }
		public required int NumberHired { get; set; }
		public required Guid Id { get; set; }
		public required string Specialty { get; set; } = null!;
	}
}
