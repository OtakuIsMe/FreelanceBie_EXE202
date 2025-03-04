using BE.src.api.domains.DTOs.User;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.controllers
{
    public sealed record RefreshTokenRequest(string RefreshToken);

    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthServ _authServ;
        public AuthController(ILogger<AuthController> logger, IAuthServ authServ)
        {
            _authServ = authServ;
            _logger = logger;
        }
        [HttpGet("Login")]
		public async Task<IActionResult> Login([FromQuery] LoginRq data)
		{
			Console.WriteLine(data.Email);
			Console.WriteLine(data.Password);
            _logger.LogInformation("Login request");
			return await _authServ.Login(data);
		}
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest req)
        {
            _logger.LogInformation("Refresh token request");
            return await _authServ.RefreshToken(req.RefreshToken);
        }
        [HttpDelete("logout")]
        public async Task<IActionResult> RevokeToken()
        {
            _logger.LogInformation("Log out");
            Guid userId = Guid.Parse(User.Claims.First(u => u.Type == "userId").Value);
            return await _authServ.RevokeRefreshToken(userId);
        }
    }
}