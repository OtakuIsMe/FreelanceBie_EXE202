using System.Text.Json;
using BE.src.api.domains.Enum;
using BE.src.api.domains.eventbus.Producers;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Service.UnitTests.Users;
public class ForgotPasswordServiceTests
{
	private readonly Mock<IUserRepo> _userRepoMock;
	private readonly Mock<IEmailServ> _emailServMock;
	private readonly Mock<ISocialProfileRepo> _socialProfileRepoMock;
	private readonly Mock<IMembershipRepo> _membershipRepoMock;
	private readonly Mock<IPostRepo> _postRepoMock;
	private readonly Mock<INotificationRepo> _notificationRepoMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly Mock<IEventBusRabbitMQProducer> _eventBusRabbitMQProducerMock;
	private readonly string _email = "test@example.com";
	private readonly UserServ _userServ;
	public ForgotPasswordServiceTests()
	{
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
	public async Task ForgotPasswordAsync_Should_ReturnSuccess_WhenUserExistsAndEmailSent()
	{
		// Arrange
		var user = new User
		{
			Username = "test",
			Email = _email,
			Password = "123",
			Role = RoleEnum.Customer
		};
		_userRepoMock.Setup(repo => repo.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(user);
		_emailServMock.Setup(emailServ => emailServ.SendVerificationEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
					  .ReturnsAsync(true);

		// Act
		var result = await _userServ.ForgotPassword(_email);
		var jsonResult = Assert.IsType<JsonResult>(result);
		var json = JsonSerializer.Serialize(jsonResult.Value);
		var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.Equal("Email sent successfully", response["Message"].ToString().Trim());
		_userRepoMock.Verify(repo => repo.GetUserByEmail(It.IsAny<string>()), Times.Once);
		_emailServMock.Verify(emailServ =>
			emailServ.SendVerificationEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
	}

	[Fact]
	public async Task ForgotPasswordAsync_Should_ThrowException_WhenUserNotFound()
	{
		// Arrange
		_userRepoMock.Setup(repo => repo.GetUserByEmail(It.IsAny<string>())).ReturnsAsync((User)null);

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.ForgotPassword(_email));
		_userRepoMock.Verify(repo => repo.GetUserByEmail(It.IsAny<string>()), Times.Once);
		_emailServMock.Verify(emailServ =>
			emailServ.SendVerificationEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
	}

	[Fact]
	public async Task ForgotPasswordAsync_Should_ThrowException_WhenEmailSendingFails()
	{
		// Arrange
		var user = new User
		{
			Username = "test",
			Email = _email,
			Password = "123",
			Role = RoleEnum.Customer
		};

		_userRepoMock.Setup(repo => repo.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(user);
		_emailServMock.Setup(emailServ =>
				emailServ.SendVerificationEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
					  .ReturnsAsync(false);

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.ForgotPassword(_email));
		_userRepoMock.Verify(repo => repo.GetUserByEmail(It.IsAny<string>()), Times.Once);
		_emailServMock.Verify(emailServ =>
			emailServ.SendVerificationEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
	}

	[Fact]
	public async Task ForgotPasswordAsync_Should_ThrowException_WhenUserRepoFails()
	{
		// Arrange
		_userRepoMock.Setup(repo => repo.GetUserByEmail(It.IsAny<string>())).ThrowsAsync(new Exception());

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.ForgotPassword(_email));
		_userRepoMock.Verify(repo => repo.GetUserByEmail(It.IsAny<string>()), Times.Once);
		_emailServMock.Verify(emailServ =>
			emailServ.SendVerificationEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
	}

	[Fact]
	public async Task ForgotPassword_Should_ThrowException_WhenEmailServiceFails()
	{
		// Arrange
		var user = new User
		{
			Username = "test",
			Email = _email,
			Password = "123",
			Role = RoleEnum.Customer
		};

		_userRepoMock.Setup(repo => repo.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(user);
		_emailServMock.Setup(emailServ =>
				emailServ.SendVerificationEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
					  .ThrowsAsync(new Exception());

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.ForgotPassword(_email));
		_userRepoMock.Verify(repo => repo.GetUserByEmail(It.IsAny<string>()), Times.Once);
		_emailServMock.Verify(emailServ =>
			emailServ.SendVerificationEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
	}
}
