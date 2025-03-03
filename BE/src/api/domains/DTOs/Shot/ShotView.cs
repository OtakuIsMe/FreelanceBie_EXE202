namespace BE.src.api.domains.DTOs.Shot
{
	public class ShotView
	{
		public required Guid Id { get; set; }
		public required string Image { get; set; }
		public required int CountView { get; set; }
		public required int CountLike { get; set; }
		public required UserShotCard User { get; set; }
		public required string Title { get; set; }
	}
}
