using BE.src.api.domains.DTOs.User;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.controllers
{
	[ApiController]
	[Route("user/")]
	public class UserController : ControllerBase
	{
		private readonly IUserServ _userServ;
		public UserController(IUserServ userServ)
		{
			_userServ = userServ;
		}
		[HttpPost("register")]
		public async Task<IActionResult> RegisterUser(UserRegisterDTO user)
		{
			return await _userServ.RegisterUser(user);
		}
		[HttpGet("get-all-users")]
		public async Task<IActionResult> GetUsers()
		{
			return await _userServ.GetAllUsers();
		}
	}
}
