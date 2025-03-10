using BE.src.api.domains.DTOs.User;
using BE.src.api.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.controllers
{
	[ApiController]
	[Route("api/v1/user")]
	public class UserController : ControllerBase
	{
		private readonly IUserServ _userServ;
		private readonly INotificationServ _notificationServ;
		private readonly ILogger<UserController> _logger;
		public UserController(IUserServ userServ, INotificationServ notificationServ, ILogger<UserController> logger)
		{
			_userServ = userServ;
			_notificationServ = notificationServ;
			_logger = logger;
		}
		[HttpPost("AddDataUser")]
		public async Task<IActionResult> AddDataUser([FromForm] UserAddData data)
		{
			_logger.LogInformation("AddDataUser");
			return await _userServ.AddDataUser(data);
		}
		[HttpGet("Login")]
		public async Task<IActionResult> Login([FromQuery] LoginRq data)
		{
			return await _userServ.Login(data);
		}
		[HttpPost("register")]
		public async Task<IActionResult> RegisterUser(UserRegisterDTO user)
		{
			_logger.LogInformation("RegisterUser");
			return await _userServ.RegisterUser(user);
		}
		[HttpGet("get-all-users")]
		public async Task<IActionResult> GetUsers()
		{
			_logger.LogInformation("GetUsers");
			return await _userServ.GetAllUsers();
		}
		[Authorize(Policy = "Customer")]
		[HttpPost("AddComment")]
		public async Task<IActionResult> AddComment([FromBody] AddCommentDTO data)
		{
			_logger.LogInformation("AddComment");
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _userServ.AddComment(userId, data);
		}
		[Authorize(Policy = "Customer")]
		[HttpPut("FollowStateChange")]
		public async Task<IActionResult> FollowChange([FromQuery] Guid Followed, [FromQuery] bool State)
		{
			_logger.LogInformation("FollowChange");
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _userServ.FollowChange(userId, Followed, State);
		}
		[Authorize(Policy = "Customer")]
		[HttpGet("SavePostShot")]
		public async Task<IActionResult> SavePostShot([FromQuery] Guid? PostId, [FromQuery] Guid? ShotId, [FromQuery] bool State)
		{
			_logger.LogInformation("SavePostShot");
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _userServ.SavePostShot(userId, PostId, ShotId, State);
		}
		[HttpGet("forgot-password")]
		public async Task<IActionResult> ForgotPassword([FromQuery] string email)
		{
			_logger.LogInformation("ForgotPassword");
			return await _userServ.ForgotPassword(email);
		}
		[HttpPost("change-password")]
		public async Task<IActionResult> ChangePassword([FromForm] UserChangePwdDTO data)
		{
			_logger.LogInformation("ChangePassword");
			return await _userServ.ChangePassword(data);
		}
		[Authorize(Policy = "Customer")]
		[HttpGet("view-notifications")]
		public async Task<IActionResult> ViewNotifications()
		{
			_logger.LogInformation("ViewNotifications");
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _notificationServ.ViewNotifications(userId);
		}
		[Authorize(Policy = "Customer")]
		[HttpGet("view-profile")]
		public async Task<IActionResult> ViewProfile()
		{
			_logger.LogInformation("ViewProfile");
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _userServ.ViewProfile(userId);
		}
		[Authorize(Policy = "Customer")]
		[HttpPut("edit-social-links")]
		public async Task<IActionResult> EditSocialLinkProfiles([FromForm] UserEditSocialLinksDTO user)
		{
			_logger.LogInformation("EditSocialLinkProfiles");
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _userServ.EditSocialLinkProfiles(userId, user);
		}
		[Authorize(Policy = "Customer")]
		[HttpPut("edit-profile")]
		public async Task<IActionResult> EditProfile([FromForm] UserEditProfileDTO user)
		{
			_logger.LogInformation("EditProfile");
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _userServ.EditProfile(userId, user);
		}
		[HttpGet("search-designers")]
		public async Task<IActionResult> SearchingDesigners([FromQuery] UserSearchingDTO userSearchingDTO)
		{
			_logger.LogInformation("SearchingDesigners");
			return await _userServ.SearchingDesigners(userSearchingDTO);
		}
		[Authorize]
		[HttpGet("getInfo")]
		public async Task<IActionResult> GetUserInfo()
		{
			_logger.LogInformation("GetUserInfo");
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _userServ.GetUserById(userId);
		}
		[Authorize(Policy = "Customer")]
		[HttpGet("nofitications-membership")]
		public async Task<IActionResult> NofiticationsMembership()
		{
			_logger.LogInformation("NofiticationsMembership");
			Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
			return await _userServ.CheckNotificationWhenPost(userId);
		}
	}
}
