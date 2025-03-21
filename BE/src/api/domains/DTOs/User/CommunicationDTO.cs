namespace BE.src.api.domains.DTOs.User
{
	public class CommunicationDTO
	{
		public required Guid MessageId { get; set; }
		public required string UserImage { get; set; } = null!;
		public required string Username { get; set; } = null!;
		public required DateTime? LastMessageTime { get; set; }
		public required string LastMessage { get; set; } = null!;
	}
}
