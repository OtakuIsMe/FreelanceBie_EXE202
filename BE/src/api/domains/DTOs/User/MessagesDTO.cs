namespace BE.src.api.domains.DTOs.User
{
	public class MessagesDTO
	{
		public required UserMes Self { get; set; }
		public required UserMes Partner { get; set; }
		public required List<Mes> Messages { get; set; }
	}

	public class UserMes
	{
		public required Guid Id { get; set; }
		public required string UserImage { get; set; }
		public required string Username { get; set; }
	}
	public class Mes
	{
		public required bool IsSelf { get; set; }
		public required string Message { get; set; }
		public required DateTime CreateAt { get; set; }
	}
}
