namespace BE.src.api.domains.DTOs.Shot
{
	public class ShotLiked
	{
		public required Guid Id { get; set; }
		public required string Title { get; set; }
		public required string Image { get; set; }
		public required string Author { get; set; }
		public required int Likes { get; set; }
		public required DateTime DatePosted { get; set; }
	}
}
