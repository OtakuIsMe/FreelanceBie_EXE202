using BE.src.api.domains.DTOs.User;
using BE.src.api.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.controllers
{
	[ApiController]
	[Route("user/")]
	public class UserController : ControllerBase
	{
		private readonly IUserServ _userServ;
		private readonly IRedisServ _redisServ;
		public UserController(IUserServ userServ, IRedisServ redisServ)
		{
			_userServ = userServ;
			_redisServ = redisServ;
		}

		[HttpPost("Login")]
		public async Task<IActionResult> Login([FromQuery] LoginRq data)
		{
			return await _userServ.Login(data);
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
