using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BE.src.api.domains.DTOs.Post;
using BE.src.api.domains.DTOs.User;
using BE.src.api.domains.Enum;
using BE.src.api.domains.eventbus.Producers;
using BE.src.api.domains.Model;
using BE.src.api.helpers;
using BE.src.api.repositories;
using BE.src.api.shared.Constant;
using BE.src.api.shared.Type;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Queue = BE.src.api.shared.Constant.EventBus;

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
		Task<IActionResult> GetUserById(Guid userId);
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
		private readonly ICacheService _cacheService;
		private readonly IEventBusRabbitMQProducer _eventBus;
		private readonly ITokenService _tokenService;
		public UserServ(IUserRepo userRepo, EmailServ emailServ, ISocialProfileRepo socialProfileRepo,
						IMembershipRepo membershipRepo, IPostRepo postRepo, INotificationRepo notificationRepo,
						ICacheService cacheService, IEventBusRabbitMQProducer eventBus, ITokenService tokenService)
		{
			_userRepo = userRepo;
			_emailServ = emailServ;
			_socialProfileRepo = socialProfileRepo;
			_membershipRepo = membershipRepo;
			_postRepo = postRepo;
			_notificationRepo = notificationRepo;
			_cacheService = cacheService;
			_eventBus = eventBus ?? throw new ApplicationException(nameof(eventBus));
			_tokenService = tokenService;
		}

		public async Task<IActionResult> Login(LoginRq data)
		{
			try
			{
				Console.WriteLine(Utils.HashObject<string>(data.Password));
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
				expires: DateTime.Now.AddHours(3),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public async Task<IActionResult> GetAllUsers()
		{
			try
			{
				var cachedUsers = await _cacheService.Get<List<User>>("all-users");
				if (cachedUsers != null)
				{
					return SuccessResp.Ok(cachedUsers);
				}

				var users = await _userRepo.GetUsers();
				if (users.Count == 0)
				{
					throw new ApplicationException("No users found");
				}

				await _cacheService.Set("all-users", users, TimeSpan.FromMinutes(10));

				return SuccessResp.Ok(users);
			}
			catch (System.Exception ex)
			{
				throw new ApplicationException(ex.Message);
			}
		}
		public async Task<IActionResult> RegisterUser(UserRegisterDTO user)
		{
			try
			{
				var existingUser = await _userRepo.GetUserByEmail(user.Email);
				if (existingUser != null)
				{
					throw new ApplicationException("User already exists");
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
					throw new ApplicationException("Failed to create user");
				}
				return SuccessResp.Created("User created successfully");
			}
			catch (System.Exception ex)
			{
				throw new ApplicationException(ex.Message);
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
						Url = userAvatar,
						IsMain = false
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
						Url = userBackground,
						IsMain = false
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
					throw new ApplicationException("Cant create user");
				}
				return SuccessResp.Created("Add data user success");
			}
			catch (System.Exception ex)
			{
				throw new ApplicationException(ex.Message);
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
					throw new ApplicationException("Cant create comment");
				}
				return SuccessResp.Created("Add comment success");
			}
			catch (System.Exception ex)
			{
				throw new ApplicationException(ex.Message);
			}
		}

		public async Task<IActionResult> FollowChange(Guid Follower, Guid Followed, bool State)
		{
			try
			{
				var redisKey = $"follow:{Follower}:{Followed}";

				var cachedFollowState = await _cacheService.Get<bool?>(redisKey);
				if (cachedFollowState.HasValue)
				{
					if (cachedFollowState.Value == State)
					{
						return SuccessResp.Ok(State ? "Already following" : "Already unfollowed");
					}
				}

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

						await _cacheService.Set(redisKey, true, TimeSpan.FromDays(15));
					}
					return SuccessResp.Created("Follow Success");
				}
				else
				{
					if (follow != null)
					{
						await _userRepo.DeleteFollow(follow);

						await _cacheService.Remove(redisKey);
					}
					return SuccessResp.Ok("Unfollow Success");
				}
			}
			catch (System.Exception ex)
			{
				throw new ApplicationException(ex.Message);
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
				throw new ApplicationException(ex.Message);
			}
		}

		public async Task<IActionResult> ForgotPassword(string email)
		{
			try
			{
				var user = await _userRepo.GetUserByEmail(email);
				if (user == null)
				{
					throw new ApplicationException("User not found");
				}

				var htmlBody = $"<h3>Click the link below to verify your email address:</h3><a href=\"https://localhost:5173/\">Verify Email</a>";

				var result = _emailServ.SendVerificationEmail(email, "Email Vertification", htmlBody);
				if (!result.Result)
				{
					throw new ApplicationException("Failed to send email");
				}
				return SuccessResp.Ok("Email sent successfully");
			}
			catch (System.Exception ex)
			{
				throw new ApplicationException(ex.Message);
			}
		}

		public async Task<IActionResult> ChangePassword(UserChangePwdDTO data)
		{
			try
			{
				if (data.NewPassword != data.ConfirmPassword)
				{
					throw new ApplicationException("Passwords do not match");
				}

				var result = await _userRepo.ChangePassword(data.Email, Utils.HashObject<string>(data.NewPassword));
				if (!result)
				{
					throw new ApplicationException("Failed to change password");
				}
				return SuccessResp.Ok("Password changed successfully");
			}
			catch (System.Exception ex)
			{
				throw new ApplicationException(ex.Message);
			}
		}

		public async Task<IActionResult> ViewProfile(Guid userId)
		{
			try
			{
				var cachedProfile = await _cacheService.Get<User>($"profile-{userId}");
				if (cachedProfile != null)
				{
					return SuccessResp.Ok(cachedProfile);
				}

				var user = await _userRepo.ViewProfileUser(userId);
				if (user == null)
				{
					throw new ApplicationException("User not found");
				}

				await _cacheService.Set($"profile-{userId}", user, TimeSpan.FromMinutes(10));

				return SuccessResp.Ok(user);
			}
			catch (System.Exception ex)
			{
				throw new ApplicationException(ex.Message);
			}
		}

		public async Task<IActionResult> EditSocialLinkProfiles(Guid userId, UserEditSocialLinksDTO user)
		{
			try
			{
				var userObj = await _userRepo.GetUserById(userId);
				if (userObj == null)
				{
					throw new ApplicationException("User not found");
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
				throw new ApplicationException(ex.Message);
			}
		}

		public async Task<IActionResult> EditProfile(Guid userId, UserEditProfileDTO user)
		{
			try
			{
				var userObj = await _userRepo.GetUserById(userId);
				if (userObj == null)
				{
					throw new ApplicationException("User not found");
				}

				userObj.Name = user.Name ?? userObj.Name;
				userObj.Phone = user.Phone ?? userObj.Phone;
				userObj.City = user.City ?? userObj.City;
				userObj.Education = user.Education ?? userObj.Education;
				userObj.Description = user.Description ?? userObj.Description;

				if (user.Image != null)
				{
					var imageUrl = await Utils.GenerateAzureUrl(MediaTypeEnum.Image, user.Image, $"user/{Utils.HashObject(userObj.Id)}");
					if (userObj.ImageVideos == null || userObj.ImageVideos.Count == 0)
					{
						var image = new ImageVideo
						{
							Type = MediaTypeEnum.Image,
							Url = imageUrl,
							IsMain = false,
							UserId = userId
						};
						await _userRepo.AddImageVideo(image);
					}
					else
					{
						var userImage = userObj.ImageVideos.First();
						userImage.Url = imageUrl;
						await _userRepo.UpdateImageVideo(userImage);
					}
				}

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
				throw new ApplicationException(ex.Message);
			}
		}

		private string GenerateCacheKey(UserSearchingDTO searchDTO)
		{
			var keyParts = new List<string>();

			if (!string.IsNullOrEmpty(searchDTO.Name)) keyParts.Add($"Name:{searchDTO.Name}");
			if (!string.IsNullOrEmpty(searchDTO.Username)) keyParts.Add($"Username:{searchDTO.Username}");
			if (!string.IsNullOrEmpty(searchDTO.Email)) keyParts.Add($"Email:{searchDTO.Email}");
			if (!string.IsNullOrEmpty(searchDTO.Phone)) keyParts.Add($"Phone:{searchDTO.Phone}");
			if (!string.IsNullOrEmpty(searchDTO.City)) keyParts.Add($"City:{searchDTO.City}");
			if (!string.IsNullOrEmpty(searchDTO.Education)) keyParts.Add($"Education:{searchDTO.Education}");

			return $"searching_designers{string.Join("|", keyParts)}";
		}

		public async Task<IActionResult> SearchingDesigners(UserSearchingDTO userSearchingDTO)
		{
			try
			{
				var searchKey = GenerateCacheKey(userSearchingDTO);

				var cachedSearching = await _cacheService.Get<List<User>>(searchKey);
				if (cachedSearching != null)
				{
					return SuccessResp.Ok(cachedSearching);
				}

				var users = await _userRepo.FindUsers(userSearchingDTO);
				if (users.Count == 0)
				{
					throw new ApplicationException("No users found");
				}

				await _cacheService.Set(searchKey, users, TimeSpan.FromMinutes(5));

				return SuccessResp.Ok(users);
			}
			catch (System.Exception ex)
			{
				throw new ApplicationException(ex.Message);
			}
		}

		public async Task<IActionResult> GetUserById(Guid userId)
		{
			try
			{
				var user = await _userRepo.GetUserById(userId);
				if (user == null)
				{
					throw new ApplicationException("Cant found user");
				}
				return SuccessResp.Ok(user);
			}
			catch (System.Exception ex)
			{
				throw new ApplicationException(ex.Message);
			}
		}

		public async Task<IActionResult> CheckNotificationWhenPost(Guid userId)
		{
			try
			{
				var cachedMemberUser = await _cacheService.Get<string>($"member-user-{userId}");
				if (cachedMemberUser is null)
				{
					var memberUser = await _membershipRepo.GetMembershipUserRegistered(userId);
					if (memberUser is null)
					{
						throw new ApplicationException("User not registered yet");
					}

					cachedMemberUser = memberUser.MembershipId.ToString();
					await _cacheService.Set($"member-user-{userId}", cachedMemberUser, TimeSpan.FromMinutes(5));
				}

				var cachedMembership = await _cacheService.Get<bool?>($"membership-{cachedMemberUser}");
				if (cachedMembership is null)
				{
					var membership = await _membershipRepo.GetMembershipById(Guid.Parse(cachedMemberUser));
					if (membership is null)
					{
						throw new ApplicationException("Membership not found");
					}

					await _cacheService.Set($"membership-{cachedMemberUser}", true, TimeSpan.FromMinutes(5));
				}

				var cachedPost = await _cacheService.Get<PostJob>("latest-post");
				if (cachedPost is null)
				{
					var post = await _postRepo.GetLatestPosts();
					if (post is null)
					{
						throw new ApplicationException("No recent posts available");
					}

					cachedPost = post;
					await _cacheService.Set("latest-post", post, TimeSpan.FromMinutes(5));
				}

				var eventMessage = new PostCreatedEvent(userId, cachedPost.Id, cachedPost.Title, DateTime.Now);
				_eventBus.Publish(Queue.PostNotificationQueue, eventMessage);

				return SuccessResp.Ok("Notification sent successfully");
			}
			catch (Exception ex)
			{
				throw new ApplicationException(ex.Message);
			}
		}
	}
}
