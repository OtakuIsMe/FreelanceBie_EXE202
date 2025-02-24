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
		private readonly INotificationServ _notificationServ;
		public UserController(IUserServ userServ, IRedisServ redisServ, INotificationServ notificationServ)
		{
			_userServ = userServ;
			_notificationServ = notificationServ;
		}
		[HttpPost("AddDataUser")]
		public async Task<IActionResult> AddDataUser([FromForm] UserAddData data)
		{
			return await _userServ.AddDataUser(data);
		}
		[HttpGet("Login")]
		public async Task<IActionResult> Login([FromQuery] LoginRq data)
		{
			Console.WriteLine(data.Email);
			Console.WriteLine(data.Password);
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
		[Authorize(Policy = "Customer")]
		[HttpPost("AddComment")]
		public async Task<IActionResult> AddComment([FromBody] AddCommentDTO data)
		{
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _userServ.AddComment(userId, data);
		}
		[Authorize(Policy = "Customer")]
		[HttpPut("FollowStateChange")]
		public async Task<IActionResult> FollowChange([FromQuery] Guid Followed, [FromQuery] bool State)
		{
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _userServ.FollowChange(userId, Followed, State);
		}
		[Authorize(Policy = "Customer")]
		[HttpGet("SavePostShot")]
		public async Task<IActionResult> SavePostShot([FromQuery] Guid? PostId, [FromQuery] Guid? ShotId, [FromQuery] bool State)
		{
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _userServ.SavePostShot(userId, PostId, ShotId, State);
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
		[Authorize(Policy = "Customer")]
		[HttpGet("view-notifications")]
		public async Task<IActionResult> ViewNotifications()
		{
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _notificationServ.ViewNotifications(userId);
		}
		[Authorize(Policy = "Customer")]
		[HttpGet("view-profile")]
		public async Task<IActionResult> ViewProfile()
		{
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _userServ.ViewProfile(userId);
		}
		[Authorize(Policy = "Customer")]
		[HttpPut("edit-social-links")]
		public async Task<IActionResult> EditSocialLinkProfiles([FromForm] UserEditSocialLinksDTO user)
		{
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _userServ.EditSocialLinkProfiles(userId, user);
		}
		[Authorize(Policy = "Customer")]
		[HttpPut("edit-profile")]
		public async Task<IActionResult> EditProfile([FromForm] UserEditProfileDTO user)
		{
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _userServ.EditProfile(userId, user);
		}
		[HttpGet("search-designers")]
		public async Task<IActionResult> SearchingDesigners([FromQuery] UserSearchingDTO userSearchingDTO)
		{
			return await _userServ.SearchingDesigners(userSearchingDTO);
		}
		[Authorize]
		[HttpGet("getInfo")]
		public async Task<IActionResult> GetUserInfo()
		{
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _userServ.GetUserById(userId);
		}
		[Authorize(Policy = "Customer")]
		[HttpGet("nofitications-membership")]
		public async Task<IActionResult> NofiticationsMembership()
		{
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _userServ.CheckNotificationWhenPost(userId);
		}
	}
}
