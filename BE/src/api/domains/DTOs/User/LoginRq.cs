namespace BE.src.api.domains.DTOs.User
{
	public class LoginRq
	{
		public required string Email { get; set; }
		public required string Password { get; set; }
	}
}
