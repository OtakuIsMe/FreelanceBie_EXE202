using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BE.src.api.domains.DTOs.User;
using BE.src.api.domains.Model;
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
	}
	public class UserServ : IUserServ
	{
		private readonly IUserRepo _userRepo;
		public UserServ(IUserRepo userRepo)
		{
			_userRepo = userRepo;
		}

		public async Task<IActionResult> Login(LoginRq data)
		{
			try
			{
				var user = await _userRepo.GetUserByEmailPassword(data.Email, data.Password);
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
	}
}
