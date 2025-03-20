using System.Security.Claims;
using BE.src.api.domains.DTOs.User;
using BE.src.api.domains.Model;
using BE.src.api.helpers;
using BE.src.api.repositories;
using BE.src.api.shared.Constant;
using BE.src.api.shared.Type;
using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.services
{
    public interface IAuthServ
    {
        Task<IActionResult> Login(LoginRq data);
        Task<IActionResult> RefreshToken(string refreshToken);
        Task<IActionResult> RevokeRefreshToken(Guid userId);
    }
	public class AuthServ : IAuthServ
	{
        private readonly ITokenService _tokenService;
        private readonly IUserRepo _userRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICacheService _cachServ;
        public AuthServ(ITokenService tokenService, IUserRepo userRepo, IHttpContextAccessor httpContextAccessor, 
                    ICacheService cachServ)
        {
            _tokenService = tokenService;
            _userRepo = userRepo;
            _httpContextAccessor = httpContextAccessor;
            _cachServ = cachServ;
        }
        public async Task<IActionResult> Login(LoginRq data)
		{
			try
			{
				Console.WriteLine(Utils.HashObject<string>(data.Password));
				var user = await _userRepo.GetUserByEmailPassword(data.Email, Utils.HashObject<string>(data.Password));
				if (user == null)
				{
					throw new ApplicationException("Login fail");
				}

				var accessToken = _tokenService.GenerateAccessToken(user);
				var refreshToken = _tokenService.GenerateRefreshToken();

				var newRefreshToken = new RefreshToken
				{
					Token = refreshToken,
					UserId = user.Id,
					ExpiresOnUtc = DateTime.UtcNow.AddDays(JWT.REFRESH_TOKEN_EXP)
				};

				await _userRepo.AddNewRefreshToken(newRefreshToken);

				var tokenResponse = new { AccessToken = accessToken, RefreshToken = refreshToken };

				await _cachServ.Set<string>($"rft:{refreshToken}",
					refreshToken, TimeSpan.FromDays(JWT.REFRESH_TOKEN_EXP));

				return SuccessResp.Ok(tokenResponse);
			}
			catch (Exception ex)
			{
				throw new ApplicationException($"Error: {ex.Message}");
			}
		}
		public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            try
            {
                var cachedToken = await _cachServ.Get<string>($"rft:{refreshToken}");
                if (string.IsNullOrEmpty(cachedToken))
                {
                    throw new ApplicationException("Redis fresh token key is invalid");
                }

                var storedToken = await _userRepo.GetRefreshToken(cachedToken);
                if (storedToken is null || storedToken.ExpiresOnUtc < DateTime.UtcNow)
                    throw new ApplicationException("Refresh token has expired");

                var newAccessToken = _tokenService.GenerateAccessToken(storedToken.User);

                storedToken.Token = _tokenService.GenerateRefreshToken();
                storedToken.ExpiresOnUtc = DateTime.UtcNow.AddDays(JWT.REFRESH_TOKEN_EXP);

                await _userRepo.UpdateNewRefreshToken(storedToken);

                var tokenResponse = new { AccessToken = newAccessToken, RefreshToken = storedToken.Token };

                return SuccessResp.Ok(tokenResponse);
            }
            catch (Exception)
            {
                throw new ApplicationException("Refresh token is invalid");
            }
        }

		public async Task<IActionResult> RevokeRefreshToken(Guid userId)
		{
			try
            {
	            if (userId != CurrentUserId())
		            throw new ApplicationException("You cannot do this action");

	            var userRftTokens = await _userRepo.GetRefreshTokens(userId);
                if (userRftTokens.Count == 0)
                    throw new ApplicationException("Cannot get user refresh tokens");

                foreach (var item in userRftTokens)
                {
                    await _cachServ.Remove($"rft:{item.Token}");
                }    

                return SuccessResp.Ok(new 
                    {
                        Count = userRftTokens.Count,    
                        IsRevoke = await _userRepo.RevokeRefreshToken(userId)
                    });    
            }
            catch (System.Exception)
            {
                throw new ApplicationException("Invalid user id");
            }
		}

        private Guid? CurrentUserId()
        {
            return Guid.TryParse(_httpContextAccessor.HttpContext?.User.Claims
                .First(u => u.Type == "userId")?.Value, out Guid parsed) ? parsed : null;
        }
	}
}
