using System.Text.Json;
using BE.src.api.domains.Enum;
using BE.src.api.domains.eventbus.Producers;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Users;
public class GetUserByIdServiceTests
{
	private readonly Mock<IUserRepo> _userRepoMock;
	private readonly Mock<IEmailServ> _emailServMock;
	private readonly Mock<ISocialProfileRepo> _socialProfileRepoMock;
	private readonly Mock<IMembershipRepo> _membershipRepoMock;
	private readonly Mock<IPostRepo> _postRepoMock;
	private readonly Mock<INotificationRepo> _notificationRepoMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly Mock<IEventBusRabbitMQProducer> _eventBusRabbitMQProducerMock;
	private readonly UserServ _userServ;
	public GetUserByIdServiceTests()
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

	private User CreateFakeUser()
	{
		return new User
		{
			Id = Guid.NewGuid(),
			Name = "User",
			Phone = "987654321",
			City = "Old City",
			Email = "user@example.com",
			Password = "123",
			Role = RoleEnum.Customer,
			Username = "User1"
			
		};
	}

	[Fact]
	public async Task GetUserByIdAsync_Should_ReturnSuccess_WhenGettingUserSuccess()
	{
		// Arrange
		var user = CreateFakeUser();
		_userRepoMock.Setup(repo => repo.GetUserById(It.IsAny<Guid>(), default))
			.ReturnsAsync(user);

		// Act
		var result = await _userServ.GetUserById(user.Id);
		var jsonResult = Assert.IsType<JsonResult>(result);
		var json = JsonSerializer.Serialize(jsonResult.Value);
		var returnedUser = JsonSerializer.Deserialize<User>(json);

		// Assert
		Assert.NotNull(returnedUser);
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.Equal("User", returnedUser.Name);
		Assert.Equal("User1", returnedUser.Username);
		_userRepoMock.Verify(repo => repo.GetUserById(It.IsAny<Guid>(), default), Times.Once);
	}

	[Fact]
	public async Task GetUserByIdAsync_Should_ThrowException_WhenGettingUserFails()
	{
		// Arrange
		var user = CreateFakeUser();
		_userRepoMock.Setup(repo => repo.GetUserById(It.IsAny<Guid>(), default))
			.ReturnsAsync((User)null);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.GetUserById(user.Id));
		_userRepoMock.Verify(repo => repo.GetUserById(It.IsAny<Guid>(), default), Times.Once);
	}

	[Fact]
	public async Task GetUserByIdAsync_Should_ThrowException_WhenGettingUserFromRepoFails()
	{
		// Arrange
		var user = CreateFakeUser();
		_userRepoMock.Setup(repo => repo.GetUserById(It.IsAny<Guid>(), default))
			.ThrowsAsync(new Exception());

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.GetUserById(user.Id));
		_userRepoMock.Verify(repo => repo.GetUserById(It.IsAny<Guid>(), default), Times.Once);
	}
}
