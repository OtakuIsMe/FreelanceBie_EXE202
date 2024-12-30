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
		Task<IActionResult> ForgotPassword(string email);
		Task<IActionResult> ChangePassword(UserChangePwdDTO data);
	}
	public class UserServ : IUserServ
	{
		private readonly IUserRepo _userRepo;
		private readonly EmailServ _emailServ;
		public UserServ(IUserRepo userRepo, EmailServ emailServ)
		{
			_userRepo = userRepo;
			_emailServ = emailServ;
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
	}
}
