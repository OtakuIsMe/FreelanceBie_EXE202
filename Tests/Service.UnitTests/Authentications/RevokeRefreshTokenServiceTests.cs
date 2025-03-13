using System.Security.Claims;
using System.Text.Json;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using BE.src.api.shared.Constant;
using DotNetEnv;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Authentications;
public class RevokeRefreshTokenServiceTests
{
	private readonly Mock<ITokenService> _tokenServiceMock;
	private readonly Mock<IUserRepo> _userRepoMock;
	private readonly Mock<IHttpContextAccessor> _contextAccessorMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly AuthServ _authServ;
	private readonly Guid _userId = Guid.NewGuid();
	private readonly string _refreshToken = "RefreshToken";
	public RevokeRefreshTokenServiceTests()
	{
		string envPath = Path.GetFullPath(Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory, "../../../../../BE/.env"));

		Env.Load(envPath);

		_tokenServiceMock = new Mock<ITokenService>();
		_userRepoMock = new Mock<IUserRepo>();
		_contextAccessorMock = new Mock<IHttpContextAccessor>();
		_cacheServiceMock = new Mock<ICacheService>();

		_authServ = new AuthServ(
			_tokenServiceMock.Object,
			_userRepoMock.Object,
			_contextAccessorMock.Object,
			_cacheServiceMock.Object);

		var claims = new List<Claim> { new Claim("userId", _userId.ToString()) };
		var identity = new ClaimsIdentity(claims);
		var principal = new ClaimsPrincipal(identity);
		var mockHttpContext = new DefaultHttpContext {User = principal};

		_contextAccessorMock.Setup(_ => _.HttpContext).Returns(mockHttpContext);
	}

	private Guid? CurrentUserId()
	{
		return Guid.TryParse(_contextAccessorMock.Object.HttpContext?.User.Claims
			.First(u => u.Type == "userId")?.Value, out Guid parsed) ? parsed : null;
	}

	[Fact]
	public async Task RevokeRefreshTokenAsync_Should_ReturnSuccess_WhenRevokingRefreshTokenSuccess()
	{
		// Arrange
		var refreshTokens = new List<RefreshToken>
		{
			new RefreshToken
			{
				Token = _refreshToken,
				ExpiresOnUtc = DateTime.UtcNow.AddDays(JWT.REFRESH_TOKEN_EXP)
			}
		};

		_userRepoMock.Setup(repo => repo.GetRefreshTokens(_userId))
			.ReturnsAsync(refreshTokens);
		foreach (var refreshToken in refreshTokens)
		{
			_cacheServiceMock.Setup(repo => repo.Remove($"rft:{refreshToken.Token}"))
				.Returns(Task.CompletedTask);
		}
		_userRepoMock.Setup(repo => repo.RevokeRefreshToken(_userId))
			.ReturnsAsync(true);

		// Act
		var result = await _authServ.RevokeRefreshToken(_userId);

		// Assert
		Assert.Equal(_userId, CurrentUserId());

		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.NotNull(jsonResult.Value);

		var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
		var json = JsonSerializer.Serialize(jsonResult.Value);
		var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json, jsonOptions);
		Assert.NotNull(response);
		Assert.Equal(refreshTokens.Count, ((JsonElement)response["Count"]).GetInt32());
		Assert.True(((JsonElement)response["IsRevoke"]).GetBoolean());

		_userRepoMock.Verify(repo => repo.GetRefreshTokens(_userId), Times.Once);
		foreach (var refreshToken in refreshTokens)
		{
			_cacheServiceMock.Verify(repo => repo.Remove($"rft:{refreshToken.Token}"), Times.AtLeast(1));
		}
		_userRepoMock.Verify(repo => repo.RevokeRefreshToken(_userId), Times.Once);
	}

	[Fact]
	public async Task RevokeRefreshTokenAsync_Should_ThrowException_WhenGettingUserHasDifferentCurrentUser()
	{
		// Arrange
		var refreshTokens = new List<RefreshToken>
		{
			new RefreshToken
			{
				Token = _refreshToken,
				ExpiresOnUtc = DateTime.UtcNow.AddDays(JWT.REFRESH_TOKEN_EXP)
			}
		};

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _authServ.RevokeRefreshToken(Guid.NewGuid()));

		_userRepoMock.Verify(repo => repo.GetRefreshTokens(_userId), Times.Never);
		foreach (var refreshToken in refreshTokens)
		{
			_cacheServiceMock.Verify(repo => repo.Remove($"rft:{refreshToken.Token}"), Times.Never);
		}
		_userRepoMock.Verify(repo => repo.RevokeRefreshToken(_userId), Times.Never);
	}

	[Fact]
	public async Task RevokeRefreshTokenAsync_Should_ThrowException_WhenGettingUserRefreshTokensFails()
	{
		// Arrange
		var refreshTokens = new List<RefreshToken>
		{
			//new RefreshToken
			//{
			//	Token = _refreshToken,
			//	ExpiresOnUtc = DateTime.UtcNow.AddDays(JWT.REFRESH_TOKEN_EXP)
			//}
		};

		_userRepoMock.Setup(repo => repo.GetRefreshTokens(_userId))
			.ReturnsAsync(refreshTokens);

		// Act

		// Assert
		Assert.Equal(_userId, CurrentUserId());
		await Assert.ThrowsAsync<ApplicationException>(() => _authServ.RevokeRefreshToken(_userId));

		_userRepoMock.Verify(repo => repo.GetRefreshTokens(_userId), Times.Once);
		foreach (var refreshToken in refreshTokens)
		{
			_cacheServiceMock.Verify(repo => repo.Remove($"rft:{refreshToken.Token}"), Times.Never);
		}
		_userRepoMock.Verify(repo => repo.RevokeRefreshToken(_userId), Times.Never);
	}

	[Fact]
	public async Task RevokeRefreshTokenAsync_Should_ThrowException_WhenRemovingCacheKeyFails()
	{
		// Arrange
		var refreshTokens = new List<RefreshToken>
		{
			new RefreshToken
			{
				Token = _refreshToken,
				ExpiresOnUtc = DateTime.UtcNow.AddDays(JWT.REFRESH_TOKEN_EXP)
			}
		};

		_userRepoMock.Setup(repo => repo.GetRefreshTokens(_userId))
			.ReturnsAsync(refreshTokens);
		foreach (var refreshToken in refreshTokens)
		{
			_cacheServiceMock.Setup(repo => repo.Remove($"rft:{refreshToken.Token}"))
				.Throws(new Exception());
		}

		// Act

		// Assert
		Assert.Equal(_userId, CurrentUserId());
		await Assert.ThrowsAsync<ApplicationException>(() => _authServ.RevokeRefreshToken(_userId));

		_userRepoMock.Verify(repo => repo.GetRefreshTokens(_userId), Times.Once);
		foreach (var refreshToken in refreshTokens)
		{
			_cacheServiceMock.Verify(repo => repo.Remove($"rft:{refreshToken.Token}"), Times.AtLeast(1));
		}
		_userRepoMock.Verify(repo => repo.RevokeRefreshToken(_userId), Times.Never);
	}
}
