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
		private readonly INotificationServ _notificationServ;
		public UserController(IUserServ userServ, IRedisServ redisServ, INotificationServ notificationServ)
		{
			_userServ = userServ;
			_redisServ = redisServ;
			_notificationServ = notificationServ;
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
		[HttpGet("forgot-password")]
		public async Task<IActionResult> ForgotPassword([FromQuery] string email)
		{
			return await _userServ.ForgotPassword(email);
		}
		[HttpPost("change-password")]
		public async Task<IActionResult> ChangePassword([FromForm] UserChangePwdDTO data)
		{
			return await _userServ.ChangePassword(data);
		}
		[HttpGet("view-notifications")]
		public async Task<IActionResult> ViewNotifications([FromQuery] Guid userId)
		{
			return await _notificationServ.ViewNotifications(userId);
		}
		[HttpGet("view-profile")]
		public async Task<IActionResult> ViewProfile([FromQuery] Guid userId)
		{
			return await _userServ.ViewProfile(userId);
		}
		[HttpPut("edit-social-links")]
		public async Task<IActionResult> EditSocialLinkProfiles([FromQuery] Guid userId, [FromForm] UserEditSocialLinksDTO user)
		{
			return await _userServ.EditSocialLinkProfiles(userId, user);
		}
		[HttpPut("edit-profile")]
		public async Task<IActionResult> EditProfile([FromQuery] Guid userId, [FromForm] UserEditProfileDTO user)
		{
			return await _userServ.EditProfile(userId, user);
		}
		[HttpGet("search-designers")]
		public async Task<IActionResult> SearchingDesigners([FromQuery] UserSearchingDTO userSearchingDTO)
		{
			return await _userServ.SearchingDesigners(userSearchingDTO);
		}
	}
}
