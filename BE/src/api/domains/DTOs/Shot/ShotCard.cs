namespace BE.src.api.domains.DTOs.Shot
{
	public class ShotCard
	{
		public required Guid Id { get; set; }
		public required string Image { get; set; }
		public required string Title { get; set; }
		public required int CountView { get; set; }
		public required int CountLike { get; set; }
		public required UserShotCard User { get; set; }
		public required List<string> Specialties { get; set; }
		public required DateTime DatePosted { get; set; }
	}
	public class UserShotCard
	{
		public required string Username { get; set; }
		public required string Image { get; set; }
	}
}
