using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.controllers
{
	[ApiController]
	[Route("user/")]
	public class UserController : ControllerBase
	{
		[HttpGet("cc")]
		public async Task<IActionResult> cc()
		{
			return Ok(new
			{
				cc = "cc"
			});
		}
	}
}
