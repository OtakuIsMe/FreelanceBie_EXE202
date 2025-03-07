using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BE.src.api.shared.Constant;
using Microsoft.IdentityModel.Tokens;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _secretKey;

    public AuthMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _secretKey = Environment.GetEnvironmentVariable("JWT_SECURITY_KEY") ?? throw new ApplicationException("JWT secret not found in environment variables.");
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        
        if(!string.IsNullOrEmpty(token))
        {
            var payload = ValidateToken(token);

            if(payload != null)
            {
                context.Items["UserId"] = payload.FindFirst("userId")?.Value;
                context.Items["Role"] = payload.FindFirst(ClaimTypes.Role)?.Value;
            }
        }

        await _next(context);
    }

    private ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = JWT.Issuer, 
                ValidAudience = JWT.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }
        catch
        {
            return null;
        }
    }   
}
