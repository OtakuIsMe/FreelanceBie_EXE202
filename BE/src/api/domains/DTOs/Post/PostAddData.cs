using BE.src.api.domains.Enum;

namespace BE.src.api.domains.DTOs.Post
{
	public class PostAddData
	{
		public required string Title { get; set; }
		public required string Description { get; set; }
		public required WorkTypeEnum WorkType { get; set; }
		public required string WorkLocation { get; set; }
		public required string CompanyName { get; set; }
		public required EmploymentTypeEnum EmploymentType { get; set; }
		public required int Experience { get; set; }
		public required Guid SpecialtyId { get; set; }
		public required IFormFile CompanyLogo { get; set; }
		public required string CompanyLink { get; set; }
		public List<IFormFile>? Files { get; set; }
		public required float Payment { get; set; }
	}
}
