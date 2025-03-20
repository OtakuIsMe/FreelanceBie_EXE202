using System.Text.Json;
using BE.src.api.domains.DTOs.User;
using BE.src.api.domains.eventbus.Producers;
using BE.src.api.repositories;
using BE.src.api.services;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Users;
public class ChangePasswordServiceTests
{
	private readonly Mock<IUserRepo> _userRepoMock;
	private readonly Mock<IEmailServ> _emailServMock;
	private readonly Mock<ISocialProfileRepo> _socialProfileRepoMock;
	private readonly Mock<IMembershipRepo> _membershipRepoMock;
	private readonly Mock<IPostRepo> _postRepoMock;
	private readonly Mock<INotificationRepo> _notificationRepoMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly Mock<IEventBusRabbitMQProducer> _eventBusRabbitMQProducerMock;
	private readonly UserChangePwdDTO _userChangePwdDTO = new UserChangePwdDTO
	{
		Email = "test@example.com",
		NewPassword = "NewPassword123",
		ConfirmPassword = "NewPassword123"
	};
	private readonly UserServ _userServ;
	public ChangePasswordServiceTests()
	{
		string envPath = Path.GetFullPath(Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory, "../../../../../BE/.env"));
		Env.Load(envPath);
		_userRepoMock = new Mock<IUserRepo>();
		_emailServMock = new Mock<IEmailServ>();
		_socialProfileRepoMock = new Mock<ISocialProfileRepo>();
		_membershipRepoMock = new Mock<IMembershipRepo>();
		_postRepoMock = new Mock<IPostRepo>();
		_notificationRepoMock = new Mock<INotificationRepo>();
		_cacheServiceMock = new Mock<ICacheService>();
		_eventBusRabbitMQProducerMock = new Mock<IEventBusRabbitMQProducer>();
		_userServ = new UserServ(
			_userRepoMock.Object,
			_emailServMock.Object,
			_socialProfileRepoMock.Object,
			_membershipRepoMock.Object,
			_postRepoMock.Object,
			_notificationRepoMock.Object,
			_cacheServiceMock.Object,
			_eventBusRabbitMQProducerMock.Object);
	}

	[Fact]
	public async Task ChangePasswordAsync_Should_ReturnSuccess_WhenPasswordChangedSuccessfully()
	{
		// Arrange
		_userRepoMock.Setup(repo => repo.ChangePassword(It.IsAny<string>(), It.IsAny<string>()))
					 .ReturnsAsync(true);

		// Act
		var result = await _userServ.ChangePassword(_userChangePwdDTO);
		var jsonResult = Assert.IsType<JsonResult>(result);
		var json = JsonSerializer.Serialize(jsonResult.Value);
		var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.Equal("Password changed successfully", response["Message"].ToString().Trim());
		_userRepoMock.Verify(repo => repo.ChangePassword(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
	}

	[Fact]
	public async Task ChangePasswordAsync_Should_ThrowException_WhenPasswordsDoNotMatch()
	{
		// Arrange

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.ChangePassword(_userChangePwdDTO));
		_userRepoMock.Verify(repo => repo.ChangePassword(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
	}

	[Fact]
	public async Task ChangePasswordAsync_Should_ThrowException_WhenChangePasswordFails()
	{
		// Arrange
		_userRepoMock.Setup(repo => repo.ChangePassword(It.IsAny<string>(), It.IsAny<string>()))
					 .ReturnsAsync(false);

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.ChangePassword(_userChangePwdDTO));
		_userRepoMock.Verify(repo => repo.ChangePassword(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
	}

	[Fact]
	public async Task ChangePasswordAsync_Should_ThrowException_WhenUserRepoThrowsException()
	{
		// Arrange
		_userRepoMock.Setup(repo => repo.ChangePassword(It.IsAny<string>(), It.IsAny<string>()))
					 .ThrowsAsync(new Exception());

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.ChangePassword(_userChangePwdDTO));
		_userRepoMock.Verify(repo => repo.ChangePassword(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
	}
}
