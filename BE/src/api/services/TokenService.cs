using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BE.src.api.domains.Model;
using BE.src.api.shared.Constant;
using DotNetEnv;
using Microsoft.IdentityModel.Tokens;

namespace BE.src.api.services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
    public class TokenService : ITokenService
    {
        private readonly string _secrectKey;

        public TokenService(IConfiguration configuration)
        {
            _secrectKey = Environment.GetEnvironmentVariable("JWT_SECURITY_KEY") ?? throw new ApplicationException("JWT secret not found in environment variables.");
        }

        public string GenerateAccessToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secrectKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new []
            {
                new Claim("userId", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: JWT.Issuer,
                audience: JWT.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(JWT.ACCESS_TOKEN_EXP),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }
    }
}