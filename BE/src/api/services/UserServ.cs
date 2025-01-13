using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BE.src.api.domains.DTOs.User;
using BE.src.api.domains.Enum;
using BE.src.api.domains.Model;
using BE.src.api.helpers;
using BE.src.api.repositories;
using BE.src.api.shared.Constant;
using BE.src.api.shared.Type;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BE.src.api.services
{
	public interface IUserServ
	{
		Task<IActionResult> Login(LoginRq data);
		Task<IActionResult> RegisterUser(UserRegisterDTO user);
		Task<IActionResult> GetAllUsers();
		Task<IActionResult> AddDataUser(UserAddData data);
		Task<IActionResult> AddComment(Guid userId, AddCommentDTO data);
		Task<IActionResult> FollowChange(Guid Follower, Guid Followed, bool State);
		Task<IActionResult> SavePostShot(Guid userId, Guid? postId, Guid? shotId, bool state);
		Task<IActionResult> ForgotPassword(string email);
		Task<IActionResult> ChangePassword(UserChangePwdDTO data);
		Task<IActionResult> ViewProfile(Guid userId);
		Task<IActionResult> EditSocialLinkProfiles(Guid userId, UserEditSocialLinksDTO user);
		Task<IActionResult> EditProfile(Guid userId, UserEditProfileDTO user);
		Task<IActionResult> SearchingDesigners(UserSearchingDTO userSearchingDTO);
		Task<IActionResult> CheckNotificationWhenPost(Guid userId);
	}
	public class UserServ : IUserServ
	{
		private readonly IUserRepo _userRepo;
		private readonly EmailServ _emailServ;
		private readonly ISocialProfileRepo _socialProfileRepo;
		private readonly IMembershipRepo _membershipRepo;
		private readonly IPostRepo _postRepo;
		private readonly INotificationRepo _notificationRepo;
		public UserServ(IUserRepo userRepo, EmailServ emailServ, ISocialProfileRepo socialProfileRepo,
						IMembershipRepo membershipRepo, IPostRepo postRepo, INotificationRepo notificationRepo)
		{
			_userRepo = userRepo;
			_emailServ = emailServ;
			_socialProfileRepo = socialProfileRepo;
			_membershipRepo = membershipRepo;
			_postRepo = postRepo;
			_notificationRepo = notificationRepo;
		}

		public async Task<IActionResult> Login(LoginRq data)
		{
			try
			{
				var user = await _userRepo.GetUserByEmailPassword(data.Email, Utils.HashObject<string>(data.Password));
				if (user == null)
				{
					return ErrorResp.BadRequest("Login fail");
				}
				string token = GenerateTokenByUser(user);
				return SuccessResp.Ok(token);
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest($"Error: {ex.Message}");
			}
		}

		private string GenerateTokenByUser(User user)
		{
			var claims = new[]
			{
				new Claim("userId", user.Id.ToString()),
				new Claim(ClaimTypes.Role, user.Role.ToString())
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWT.SecretKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: JWT.Issuer,
				audience: JWT.Audience,
				claims: claims,
				expires: DateTime.Now.AddHours(1),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
		public async Task<IActionResult> GetAllUsers()
		{
			try
			{
				var users = await _userRepo.GetUsers();
				if (users.Count == 0)
				{
					return ErrorResp.NotFound("No users found");
				}
				return SuccessResp.Ok(users);
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}
		public async Task<IActionResult> RegisterUser(UserRegisterDTO user)
		{
			try
			{
				var existingUser = await _userRepo.GetUserByEmail(user.Email);
				if (existingUser != null)
				{
					return ErrorResp.BadRequest("User already exists");
				}

				var newUser = new User
				{
					Name = user.Name,
					Username = user.UserName,
					Email = user.Email,
					Password = Utils.HashObject<string>(user.Password),
					Role = RoleEnum.Customer,
					CreateAt = DateTime.Now,
					UpdateAt = DateTime.Now
				};

				var result = await _userRepo.CreateUser(newUser);
				if (!result)
				{
					return ErrorResp.BadRequest("Failed to create user");
				}
				return SuccessResp.Created("User created successfully");
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}

		public async Task<IActionResult> AddDataUser(UserAddData data)
		{
			try
			{
				User newUser = new()
				{
					Name = data.Name,
					Username = data.Username,
					Email = data.Email,
					Password = Utils.HashObject(data.Password),
					Role = data.Role,
					Phone = data.Phone,
					City = data.City,
					Education = data.Education,
					Description = data.Description,
					DOB = data.DOB
				};
				List<ImageVideo> imageVideos = [];
				if (data.Avatar != null)
				{
					var userAvatar = await Utils.GenerateAzureUrl(MediaTypeEnum.Image,
										data.Avatar, $"user/avatar/{Utils.HashObject(newUser.Id)}");
					ImageVideo newImageVideo = new()
					{
						Type = MediaTypeEnum.Image,
						Url = userAvatar
					};
					imageVideos.Add(newImageVideo);
				}
				if (data.Background != null)
				{
					var userBackground = await Utils.GenerateAzureUrl(MediaTypeEnum.Image,
										data.Background, $"user/background/{Utils.HashObject(newUser.Id)}");
					ImageVideo newImageVideo = new()
					{
						Type = MediaTypeEnum.Image,
						Url = userBackground
					};
					imageVideos.Add(newImageVideo);
				}
				newUser.ImageVideos = imageVideos;
				if (data.SocialProfiles != null && data.SocialProfiles.Count != 0)
				{
					List<SocialProfile> socialProfiles = [];
					foreach (SocialProfileDTO socialProfileDTO in data.SocialProfiles)
					{
						SocialProfile newSocialProfile = new()
						{
							Type = socialProfileDTO.Type,
							Linked = socialProfileDTO.Linked
						};
						socialProfiles.Add(newSocialProfile);
					}
					newUser.SocialProfiles = socialProfiles;
				}
				bool isCreated = await _userRepo.CreateUser(newUser);
				if (!isCreated)
				{
					return ErrorResp.BadRequest("Cant create user");
				}
				return SuccessResp.Created("Add data user success");
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}

		public async Task<IActionResult> AddComment(Guid userId, AddCommentDTO data)
		{
			try
			{
				Comment comment = new()
				{
					UserId = userId,
					ShotId = data.ShotId,
					Description = data.Description
				};
				var isCreated = await _userRepo.AddComment(comment);
				if (!isCreated)
				{
					return ErrorResp.BadRequest("Cant create comment");
				}
				return SuccessResp.Created("Add comment success");
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}

		public async Task<IActionResult> FollowChange(Guid Follower, Guid Followed, bool State)
		{
			try
			{
				var follow = await _userRepo.GetFollow(Follower, Followed);
				if (State)
				{
					if (follow == null)
					{
						Follow newFollow = new()
						{
							FollowedId = Followed,
							FollowingId = Follower
						};
						await _userRepo.CreateFollow(newFollow);
					}
					return SuccessResp.Created("Follow Success");
				}
				else
				{
					if (follow != null)
					{
						await _userRepo.DeleteFollow(follow);
					}
					return SuccessResp.Ok("Unfollow Success");
				}
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}

		public async Task<IActionResult> SavePostShot(Guid userId, Guid? postId, Guid? shotId, bool state)
		{
			try
			{
				var save = await _userRepo.GetUserSave(userId);
				if (state)
				{
					if (save == null)
					{
						Save newSave = new()
						{
							UserId = userId,
							PostId = postId,
							ShotId = shotId
						};
						await _userRepo.CreateSave(newSave);
					}
					else
					{
						if (postId != null)
						{
							save.PostId = postId;
						}
						if (shotId != null)
						{
							save.ShotId = shotId;
						}
						await _userRepo.UpdateSave(save);
					}
					return SuccessResp.Ok("Save success");
				}
				else
				{
					if (save != null)
					{
						if (postId != null)
						{
							save.PostId = null;
						}
						if (shotId != null)
						{
							save.ShotId = null;
						}
						if (save.PostId == null && save.ShotId == null)
						{
							await _userRepo.DeleteSave(save);
						}
						else
						{
							await _userRepo.UpdateSave(save);
						}
					}
					return SuccessResp.Ok("Unsave success");
				}
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}

		public async Task<IActionResult> ForgotPassword(string email)
		{
			try
			{
				var user = await _userRepo.GetUserByEmail(email);
				if (user == null)
				{
					return ErrorResp.NotFound("User not found");
				}

				var htmlBody = $"<h3>Click the link below to verify your email address:</h3><a href=\"https://localhost:5173/\">Verify Email</a>";

				var result = _emailServ.SendVerificationEmail(email, "Email Vertification", htmlBody);
				if (!result.Result)
				{
					return ErrorResp.BadRequest("Failed to send email");
				}
				return SuccessResp.Ok("Email sent successfully");
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}

		public async Task<IActionResult> ChangePassword(UserChangePwdDTO data)
		{
			try
			{
				if (data.NewPassword != data.ConfirmPassword)
				{
					return ErrorResp.BadRequest("Passwords do not match");
				}

				var result = await _userRepo.ChangePassword(data.Email, Utils.HashObject<string>(data.NewPassword));
				if (!result)
				{
					return ErrorResp.BadRequest("Failed to change password");
				}
				return SuccessResp.Ok("Password changed successfully");
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}

		public async Task<IActionResult> ViewProfile(Guid userId)
		{
			try
			{
				var user = await _userRepo.ViewProfileUser(userId);
				if (user == null)
				{
					return ErrorResp.NotFound("User not found");
				}
				return SuccessResp.Ok(user);
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}

		public async Task<IActionResult> EditSocialLinkProfiles(Guid userId, UserEditSocialLinksDTO user)
		{
			try
			{
				var userObj = await _userRepo.GetUserById(userId);
				if (userObj == null)
				{
					return ErrorResp.NotFound("User not found");
				}

				var existingSocialProfiles = await _socialProfileRepo.GetSocialProfiles(userId);

				foreach (TypeSocialEnum type in Enum.GetValues(typeof(TypeSocialEnum)))
				{
					var profile = existingSocialProfiles.FirstOrDefault(sp => sp.Type == type);

					if (profile == null)
					{
						var newProfile = new SocialProfile
						{
							UserId = userId,
							Type = type,
							Linked = user.GetLinkByType(type) ?? string.Empty
						};
						await _socialProfileRepo.AddNewSocialProfile(newProfile);
					}
					else
					{
						profile.Linked = user.GetLinkByType(type) ?? profile.Linked;
						await _socialProfileRepo.EditSocialProfile(profile);
					}
				}

				return SuccessResp.Ok("Social profiles updated successfully.");
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}

		public async Task<IActionResult> EditProfile(Guid userId, UserEditProfileDTO user)
		{
			try
			{
				var userObj = await _userRepo.GetUserById(userId);
				if (userObj == null)
				{
					return ErrorResp.NotFound("User not found");
				}

				userObj.Name = user.Name ?? userObj.Name;
				userObj.Phone = user.Phone ?? userObj.Phone;
				userObj.City = user.City ?? userObj.City;
				userObj.Education = user.Education ?? userObj.Education;
				userObj.Description = user.Description ?? userObj.Description;

				if (!string.IsNullOrEmpty(user.DOB))
				{
					if (DateOnly.TryParse(user.DOB, out var parsedDOB))
					{
						userObj.DOB = parsedDOB;
					}
				}
				else
				{
					userObj.DOB = userObj.DOB;
				}


				await _userRepo.EditProfile(userObj);
				return SuccessResp.Ok("Profile updated successfully.");
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}

		public async Task<IActionResult> SearchingDesigners(UserSearchingDTO userSearchingDTO)
		{
			try
			{
				var users = await _userRepo.FindUsers(userSearchingDTO);
				if (users.Count == 0)
				{
					return ErrorResp.NotFound("No users found");
				}
				return SuccessResp.Ok(users);
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}

		public async Task<IActionResult> CheckNotificationWhenPost(Guid userId)
		{
			try
			{
				var memberUser = await _membershipRepo.GetMembershipUserRegistered(userId);
				if (memberUser == null)
				{
					return ErrorResp.NotFound("User not registered yet");
				}

				var membership = await _membershipRepo.GetMembershipById(memberUser.MembershipId);
				if (membership == null)
				{
					return ErrorResp.NotFound("Membership not found");
				}

				var post = await _postRepo.GetLatestPosts();
				if (post == null)
				{
					return ErrorResp.NotFound("No recent posts available");
				}

				var newNotification = new Notification
				{
					Title = post.Title,
					Message = $"A new post titled '{post.Title}' has been published. Check it out!",
					UserId = userId,
					CreateAt = DateTime.Now,
					UpdateAt = DateTime.Now
				};

				var result = await _notificationRepo.AddNotification(newNotification);
				if (!result)
				{
					return ErrorResp.BadRequest("Failed to send notification");
				}

				return SuccessResp.Ok("Notification sent successfully");
			}
			catch (Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}
	}
}
