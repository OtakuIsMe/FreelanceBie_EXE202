namespace BE.src.api.domains.DTOs.Shot
{
	public class ShotCard
	{

		public required string Image;
		public required int CountView;
		public required int CountLike;
		public required UserShotCard User;
	}
	public class UserShotCard
	{
		public required string Username;
		public required string Image;
	}
}
