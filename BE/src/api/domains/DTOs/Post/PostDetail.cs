using BE.src.api.domains.Enum;

namespace BE.src.api.domains.DTOs.Post
{
	public class PostDetail
	{
		public required string Title { get; set; }
		public required string CompanyLogo { get; set; }
		public required string CompanyName { get; set; }
		public required string WorkLocation { get; set; }
		public required WorkTypeEnum WorkType { get; set; }
		public required EmploymentTypeEnum EmploymentType { get; set; }
		public required int Experience { get; set; }
		public required string Description { get; set; }
		public IEnumerable<AttachmentPost>? AttachmentPosts { get; set; }
		public UserPost? User { get; set; }
	}
	public class AttachmentPost
	{
		public required Guid Id { get; set; }
		public required string Name { get; set; }
		public required string Type { get; set; }
	}
	public class UserPost
	{
		public required bool IsApplied { get; set; }
		public required bool IsSaved { get; set; }
	}
}
