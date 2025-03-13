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
public class RefreshTokenServiceTests
{
	private readonly Mock<ITokenService> _tokenServiceMock;
	private readonly Mock<IUserRepo> _userRepoMock;
	private readonly Mock<IHttpContextAccessor> _contextAccessorMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly AuthServ _authServ;
	private readonly string _newAccessToken = "NewAccessToken";
	private readonly string _refreshToken = "RefreshToken";
	private readonly string _newRefreshToken = "NewRefreshToken";
	public RefreshTokenServiceTests()
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
	}

	[Fact]
	public async Task RefreshTokenAsync_Should_ReturnSuccess_WhenGeneratingRefreshTokenSuccess()
	{
		// Arrange
		var storedRefreshToken = new RefreshToken
		{
			Token = _refreshToken,
			ExpiresOnUtc = DateTime.UtcNow.AddDays(JWT.REFRESH_TOKEN_EXP)
		};

		_cacheServiceMock.Setup(cache => cache.Get<string>($"rft:{_refreshToken}"))
			.ReturnsAsync(_refreshToken);
		_userRepoMock.Setup(repo => repo.GetRefreshToken(_refreshToken))
			.ReturnsAsync(storedRefreshToken);
		_tokenServiceMock.Setup(token => token.GenerateAccessToken(storedRefreshToken.User))
			.Returns(_newAccessToken);
		_tokenServiceMock.Setup(token => token.GenerateRefreshToken())
			.Returns(_newRefreshToken);
		_userRepoMock.Setup(repo => repo.UpdateNewRefreshToken(It.IsAny<RefreshToken>()))
			.ReturnsAsync(true);

		// Act
		var result = await _authServ.RefreshToken(_refreshToken);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.NotNull(jsonResult.Value);

		var json = JsonSerializer.Serialize(jsonResult.Value);
		var tokenResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
		Assert.NotNull(tokenResponse);

		Assert.Equal("NewAccessToken", tokenResponse["AccessToken"]);
		Assert.Equal("NewRefreshToken", tokenResponse["RefreshToken"]);

		_cacheServiceMock.Verify(cache => cache.Get<string>($"rft:{_refreshToken}"), Times.Once);
		_userRepoMock.Verify(repo => repo.GetRefreshToken(_refreshToken), Times.Once);
		_tokenServiceMock.Verify(token => token.GenerateAccessToken(storedRefreshToken.User), Times.Once);
		_tokenServiceMock.Verify(token => token.GenerateRefreshToken(), Times.Once);
		_userRepoMock.Verify(repo => repo.UpdateNewRefreshToken(It.IsAny<RefreshToken>()), Times.Once);
	}

	[Fact]
	public async Task RefreshTokenAsync_Should_ThrowException_WhenGettingRefreshTokenCacheFails()
	{
		// Arrange
		var storedRefreshToken = new RefreshToken
		{
			Token = _refreshToken,
			ExpiresOnUtc = DateTime.UtcNow.AddDays(JWT.REFRESH_TOKEN_EXP)
		};

		_cacheServiceMock.Setup(cache => cache.Get<string>($"rft:{_refreshToken}"))
			.ReturnsAsync(string.Empty);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _authServ.RefreshToken(_refreshToken));

		_cacheServiceMock.Verify(cache => cache.Get<string>($"rft:{_refreshToken}"), Times.Once);
		_userRepoMock.Verify(repo => repo.GetRefreshToken(_refreshToken), Times.Never);
		_tokenServiceMock.Verify(token => token.GenerateAccessToken(storedRefreshToken.User), Times.Never);
		_tokenServiceMock.Verify(token => token.GenerateRefreshToken(), Times.Never);
		_userRepoMock.Verify(repo => repo.UpdateNewRefreshToken(It.IsAny<RefreshToken>()), Times.Never);
	}

	[Fact]
	public async Task RefreshTokenAsync_Should_ThrowException_WhenGettingRefreshTokenIsNullOrExpires()
	{
		// Arrange
		var storedRefreshToken = new RefreshToken
		{
			//Token = _refreshToken,
			//ExpiresOnUtc = DateTime.UtcNow.AddDays(JWT.REFRESH_TOKEN_EXP)
		};

		_cacheServiceMock.Setup(cache => cache.Get<string>($"rft:{_refreshToken}"))
			.ReturnsAsync(_refreshToken);
		_userRepoMock.Setup(repo => repo.GetRefreshToken(_refreshToken))
			.ReturnsAsync(storedRefreshToken);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _authServ.RefreshToken(_refreshToken));

		_cacheServiceMock.Verify(cache => cache.Get<string>($"rft:{_refreshToken}"), Times.Once);
		_userRepoMock.Verify(repo => repo.GetRefreshToken(_refreshToken), Times.Once);
		_tokenServiceMock.Verify(token => token.GenerateAccessToken(storedRefreshToken.User), Times.Never);
		_tokenServiceMock.Verify(token => token.GenerateRefreshToken(), Times.Never);
		_userRepoMock.Verify(repo => repo.UpdateNewRefreshToken(It.IsAny<RefreshToken>()), Times.Never);
	}

	[Fact]
	public async Task RefreshTokenAsync_Should_ThrowException_WhenUpdatingNewRefreshTokenFails()
	{
		// Arrange
		var storedRefreshToken = new RefreshToken
		{
			Token = _refreshToken,
			ExpiresOnUtc = DateTime.UtcNow.AddDays(JWT.REFRESH_TOKEN_EXP)
		};

		_cacheServiceMock.Setup(cache => cache.Get<string>($"rft:{_refreshToken}"))
			.ReturnsAsync(_refreshToken);
		_userRepoMock.Setup(repo => repo.GetRefreshToken(_refreshToken))
			.ReturnsAsync(storedRefreshToken);
		_tokenServiceMock.Setup(token => token.GenerateAccessToken(storedRefreshToken.User))
			.Returns(_newAccessToken);
		_tokenServiceMock.Setup(token => token.GenerateRefreshToken())
			.Returns(_newRefreshToken);
		_userRepoMock.Setup(repo => repo.UpdateNewRefreshToken(It.IsAny<RefreshToken>()))
			.Throws(new Exception());

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _authServ.RefreshToken(_refreshToken));

		_cacheServiceMock.Verify(cache => cache.Get<string>($"rft:{_refreshToken}"), Times.Once);
		_userRepoMock.Verify(repo => repo.GetRefreshToken(_refreshToken), Times.Once);
		_tokenServiceMock.Verify(token => token.GenerateAccessToken(storedRefreshToken.User), Times.Once);
		_tokenServiceMock.Verify(token => token.GenerateRefreshToken(), Times.Once);
		_userRepoMock.Verify(repo => repo.UpdateNewRefreshToken(It.IsAny<RefreshToken>()), Times.Once);
	}
}
