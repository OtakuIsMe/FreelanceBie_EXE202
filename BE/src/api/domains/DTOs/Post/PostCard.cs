namespace BE.src.api.domains.DTOs.Post
{
	public class PostCard
	{
		public required Guid Id { get; set; }
		public required string CompanyLogo { get; set; }
		public required string CompanyName { get; set; }
		public required string Title { get; set; }
		public required double LastPosted { get; set; }
		public required string WorkLocation { get; set; }
	}
}
