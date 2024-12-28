namespace BE.src.api.domains.DTOs.User
{
	public class UserImageTest
	{
		public required IFormFile Image { get; set; }
		public required string UserName { get; set; }
	}
}
