using BE.src.api.domains.Enum;

namespace BE.src.api.domains.DTOs.ElasticSearch
{
    public class PostsDTO
    {
        public string? Title { get; set; }
		public WorkTypeEnum WorkType { get; set; }
		public string? WorkLocation { get; set; }
		public string? CompanyName { get; set; }
		public EmploymentTypeEnum EmploymentType { get; set; }
		public int Experience { get; set; }
    }
}