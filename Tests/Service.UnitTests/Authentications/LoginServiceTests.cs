using System.Text.Json;
using BE.src.api.domains.DTOs.User;
using BE.src.api.domains.Enum;
using BE.src.api.domains.Model;
using BE.src.api.helpers;
using BE.src.api.repositories;
using BE.src.api.services;
using BE.src.api.shared.Constant;
using DotNetEnv;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Authentications;
public class LoginServiceTests
{
	private readonly Mock<ITokenService> _tokenServiceMock;
	private readonly Mock<IUserRepo> _userRepoMock;
	private readonly Mock<IHttpContextAccessor> _contextAccessorMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly AuthServ _authServ;
	private readonly string _accessToken = "AccessToken";
	private readonly string _refreshToken = "RefreshToken";
	private readonly string _email = "customer@example.com";
	private readonly string _password = "123";
	public LoginServiceTests()
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
	public async Task LoginAsync_Should_ReturnSuccess_WhenLoginSuccess()
	{
		// Arrange
		var user = new User
		{
			Email = _email,
			Password = Utils.HashObject<string>(_password),
			Role = RoleEnum.Customer,
			Username = "customer"
		};

		var login = new LoginRq
		{
			Email = _email,
			Password = _password
		};

		_tokenServiceMock.Setup(service => service.GenerateAccessToken(It.IsAny<User>()))
			.Returns(_accessToken);
		_tokenServiceMock.Setup(service => service.GenerateRefreshToken())
			.Returns(_refreshToken);
		_userRepoMock.Setup(repo => repo.GetUserByEmailPassword(login.Email, Utils.HashObject<string>(login.Password)))
			.ReturnsAsync(user);
		_userRepoMock.Setup(repo => repo.AddNewRefreshToken(It.IsAny<RefreshToken>()))
			.ReturnsAsync(true);
		_cacheServiceMock.Setup(cache =>
				cache.Set<string>($"rft:{_refreshToken}", _refreshToken, TimeSpan.FromDays(JWT.REFRESH_TOKEN_EXP)))
			.Returns(Task.CompletedTask);

		// Act
		var result = await _authServ.Login(login);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.NotNull(jsonResult.Value);

		var json = JsonSerializer.Serialize(jsonResult.Value);
		var tokenResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

		Assert.NotNull(tokenResponse);
		Assert.Equal(_accessToken, tokenResponse["AccessToken"]);
		Assert.Equal(_refreshToken, tokenResponse["RefreshToken"]);

		_userRepoMock.Verify(repo => repo.GetUserByEmailPassword(login.Email, Utils.HashObject<string>(login.Password)), Times.Once);
		_userRepoMock.Verify(repo => repo.AddNewRefreshToken(It.IsAny<RefreshToken>()), Times.Once);
		_tokenServiceMock.Verify(service => service.GenerateAccessToken(It.IsAny<User>()), Times.Once);
		_tokenServiceMock.Verify(service => service.GenerateRefreshToken(), Times.Once);
		_cacheServiceMock.Verify(cache =>
			cache.Set<string>($"rft:{_refreshToken}", _refreshToken, TimeSpan.FromDays(JWT.REFRESH_TOKEN_EXP)), Times.Once);
	}

	[Fact]
	public async Task LoginAsync_Should_ThrowException_WhenLoginFails()
	{
		// Arrange
		var login = new LoginRq
		{
			Email = _email,
			Password = _password
		};

		_userRepoMock.Setup(repo => repo.GetUserByEmailPassword(login.Email, Utils.HashObject<string>(login.Password)))
			.ReturnsAsync((User)null);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _authServ.Login(login));

		_userRepoMock.Verify(repo => repo.GetUserByEmailPassword(login.Email, Utils.HashObject<string>(login.Password)), Times.Once);
		_userRepoMock.Verify(repo => repo.AddNewRefreshToken(It.IsAny<RefreshToken>()), Times.Never);
		_tokenServiceMock.Verify(service => service.GenerateAccessToken(It.IsAny<User>()), Times.Never);
		_tokenServiceMock.Verify(service => service.GenerateRefreshToken(), Times.Never);
		_cacheServiceMock.Verify(cache =>
			cache.Set<string>($"rft:{_refreshToken}", _refreshToken, TimeSpan.FromDays(JWT.REFRESH_TOKEN_EXP)), Times.Never);
	}

	[Fact]
	public async Task LoginAsync_Should_ThrowException_WhenAddNewRefreshTokenFails()
	{
		// Arrange
		var user = new User
		{
			Email = _email,
			Password = Utils.HashObject<string>(_password),
			Role = RoleEnum.Customer,
			Username = "customer"
		};

		var login = new LoginRq
		{
			Email = _email,
			Password = _password
		};

		_tokenServiceMock.Setup(service => service.GenerateAccessToken(It.IsAny<User>()))
			.Returns(_accessToken);
		_tokenServiceMock.Setup(service => service.GenerateRefreshToken())
			.Returns(_refreshToken);
		_userRepoMock.Setup(repo => repo.GetUserByEmailPassword(login.Email, Utils.HashObject<string>(login.Password)))
			.ReturnsAsync(user);
		_userRepoMock.Setup(repo => repo.AddNewRefreshToken(It.IsAny<RefreshToken>()))
			.Throws(new Exception());

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _authServ.Login(login));

		_userRepoMock.Verify(repo => repo.GetUserByEmailPassword(login.Email, Utils.HashObject<string>(login.Password)), Times.Once);
		_tokenServiceMock.Verify(service => service.GenerateAccessToken(It.IsAny<User>()), Times.Once);
		_tokenServiceMock.Verify(service => service.GenerateRefreshToken(), Times.Once);
		_userRepoMock.Verify(repo => repo.AddNewRefreshToken(It.IsAny<RefreshToken>()), Times.Once);
		_cacheServiceMock.Verify(cache =>
			cache.Set<string>($"rft:{_refreshToken}", _refreshToken, TimeSpan.FromDays(JWT.REFRESH_TOKEN_EXP)), Times.Never);
	}

	[Fact]
	public async Task LoginAsync_Should_ThrowException_WhenSettingNewRefreshTokenCacheFails()
	{
		// Arrange
		var user = new User
		{
			Email = _email,
			Password = Utils.HashObject<string>(_password),
			Role = RoleEnum.Customer,
			Username = "customer"
		};

		var login = new LoginRq
		{
			Email = _email,
			Password = _password
		};

		_tokenServiceMock.Setup(service => service.GenerateAccessToken(It.IsAny<User>()))
			.Returns(_accessToken);
		_tokenServiceMock.Setup(service => service.GenerateRefreshToken())
			.Returns(_refreshToken);
		_userRepoMock.Setup(repo => repo.GetUserByEmailPassword(login.Email, Utils.HashObject<string>(login.Password)))
			.ReturnsAsync(user);
		_userRepoMock.Setup(repo => repo.AddNewRefreshToken(It.IsAny<RefreshToken>()))
			.ReturnsAsync(true);
		_cacheServiceMock.Setup(cache =>
				cache.Set<string>($"rft:{_refreshToken}", _refreshToken, TimeSpan.FromDays(JWT.REFRESH_TOKEN_EXP)))
			.Throws(new Exception());

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _authServ.Login(login));

		_userRepoMock.Verify(repo => repo.GetUserByEmailPassword(login.Email, Utils.HashObject<string>(login.Password)), Times.Once);
		_userRepoMock.Verify(repo => repo.AddNewRefreshToken(It.IsAny<RefreshToken>()), Times.Once);
		_tokenServiceMock.Verify(service => service.GenerateAccessToken(It.IsAny<User>()), Times.Once);
		_tokenServiceMock.Verify(service => service.GenerateRefreshToken(), Times.Once);
		_cacheServiceMock.Verify(cache =>
			cache.Set<string>($"rft:{_refreshToken}", _refreshToken, TimeSpan.FromDays(JWT.REFRESH_TOKEN_EXP)), Times.Once);
	}

	[Fact]
	public async Task LoginAsync_Should_ReturnNull_WhenAccessOrRefreshTokenAreNull()
	{
		// Arrange
		var user = new User
		{
			Email = _email,
			Password = Utils.HashObject<string>(_password),
			Role = RoleEnum.Customer,
			Username = "customer"
		};

		var login = new LoginRq
		{
			Email = _email,
			Password = _password
		};

		_userRepoMock.Setup(repo => repo.GetUserByEmailPassword(login.Email, Utils.HashObject<string>(login.Password)))
			.ReturnsAsync(user);
		_tokenServiceMock.Setup(service => service.GenerateAccessToken(It.IsAny<User>()))
			.Returns(String.Empty);
		_tokenServiceMock.Setup(service => service.GenerateRefreshToken())
			.Returns(String.Empty);
		_userRepoMock.Setup(repo => repo.AddNewRefreshToken(It.IsAny<RefreshToken>()))
			.ReturnsAsync(true);

		// Act
		var result = await _authServ.Login(login);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.NotNull(jsonResult.Value);

		var json = JsonSerializer.Serialize(jsonResult.Value);
		var tokenResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
		Assert.NotNull(tokenResponse);

		Assert.Equal(string.Empty, tokenResponse["AccessToken"]);
		Assert.Equal(string.Empty,tokenResponse["RefreshToken"]);

		_userRepoMock.Verify(repo => repo.GetUserByEmailPassword(login.Email, Utils.HashObject<string>(login.Password)), Times.Once);
		_tokenServiceMock.Verify(service => service.GenerateAccessToken(It.IsAny<User>()), Times.Once);
		_tokenServiceMock.Verify(service => service.GenerateRefreshToken(), Times.Once);
		_userRepoMock.Verify(repo => repo.AddNewRefreshToken(It.IsAny<RefreshToken>()), Times.Once);
		_cacheServiceMock.Verify(cache =>
			cache.Set<string>($"rft:{_refreshToken}", _refreshToken, TimeSpan.FromDays(JWT.REFRESH_TOKEN_EXP)), Times.Never);
	}
}
