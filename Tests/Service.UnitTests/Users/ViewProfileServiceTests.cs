using System.Text.Json;
using BE.src.api.domains.Enum;
using BE.src.api.domains.eventbus.Producers;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Users;
public class ViewProfileServiceTests
{
	private readonly Mock<IUserRepo> _userRepoMock;
	private readonly Mock<IEmailServ> _emailServMock;
	private readonly Mock<ISocialProfileRepo> _socialProfileRepoMock;
	private readonly Mock<IMembershipRepo> _membershipRepoMock;
	private readonly Mock<IPostRepo> _postRepoMock;
	private readonly Mock<INotificationRepo> _notificationRepoMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly Mock<IEventBusRabbitMQProducer> _eventBusRabbitMQProducerMock;
	private readonly User _user = new User
	{
		Id = Guid.NewGuid(),
		Username = "CachedUser",
		Email = "user@example.com",
		Password = "123",
		Role = RoleEnum.Customer
	};
	private readonly UserServ _userServ;
	public ViewProfileServiceTests()
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
	public async Task ViewProfileAsync_Should_ReturnSuccess_WhenProfileExistsInCache()
	{
		// Arrange
		_cacheServiceMock.Setup(cache => cache.Get<User>(It.IsAny<string>()))
						 .ReturnsAsync(_user);

		// Act
		var result = await _userServ.ViewProfile(_user.Id);
		var jsonResult = Assert.IsType<JsonResult>(result);
		var json = JsonSerializer.Serialize(jsonResult.Value);
		var response = JsonSerializer.Deserialize<User>(json);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.Equal(_user.Username, response.Username);
		_cacheServiceMock.Verify(cache => cache.Get<User>(It.IsAny<string>()), Times.Once);
		_userRepoMock.Verify(repo => repo.ViewProfileUser(It.IsAny<Guid>(), default), Times.Never);
		_cacheServiceMock.Verify(cache => cache.Set(It.IsAny<string>(), It.IsAny<User>(), It.IsAny<TimeSpan>()), Times.Never);
	}

	[Fact]
	public async Task ViewProfileAsync_Should_ReturnSuccess_WhenGettingFromDatabaseNotInCached()
	{
		// Arrange
		_cacheServiceMock.Setup(cache => cache.Get<User>(It.IsAny<string>()))
						 .ReturnsAsync((User)null);
		_userRepoMock.Setup(repo => repo.ViewProfileUser(It.IsAny<Guid>(), default))
					 .ReturnsAsync(_user);
		_cacheServiceMock.Setup(cache => cache.Set(It.IsAny<string>(), It.IsAny<User>(), TimeSpan.FromMinutes(10)))
						 .Returns(Task.CompletedTask);

		// Act
		var result = await _userServ.ViewProfile(_user.Id);
		var jsonResult = Assert.IsType<JsonResult>(result);
		var json = JsonSerializer.Serialize(jsonResult.Value);
		var response = JsonSerializer.Deserialize<User>(json);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.Equal(_user.Username, response.Username);
		_cacheServiceMock.Verify(cache => cache.Get<User>(It.IsAny<string>()), Times.Once);
		_userRepoMock.Verify(repo => repo.ViewProfileUser(It.IsAny<Guid>(), default), Times.Once);
		_cacheServiceMock.Verify(cache => cache.Set(It.IsAny<string>(), It.IsAny<User>(), TimeSpan.FromMinutes(10)), Times.Once);
	}

	[Fact]
	public async Task ViewProfileAsync_Should_ThrowException_WhenUserNotFound()
	{
		// Arrange
		_cacheServiceMock.Setup(cache => cache.Get<User>(It.IsAny<string>()))
						 .ReturnsAsync((User)null);
		_userRepoMock.Setup(repo => repo.ViewProfileUser(It.IsAny<Guid>(), default))
					 .ReturnsAsync((User)null);

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.ViewProfile(_user.Id));
		_cacheServiceMock.Verify(cache => cache.Get<User>(It.IsAny<string>()), Times.Once);
		_userRepoMock.Verify(repo => repo.ViewProfileUser(It.IsAny<Guid>(), default), Times.Once);
		_cacheServiceMock.Verify(cache => cache.Set(It.IsAny<string>(), It.IsAny<User>(), It.IsAny<TimeSpan>()), Times.Never);
	}

	[Fact]
	public async Task ViewProfileAsync_Should_ThrowException_WhenUserRepoThrowsException()
	{
		// Arrange
			_cacheServiceMock.Setup(cache => cache.Get<User>(It.IsAny<string>()))
							 .ReturnsAsync((User)null);
		_userRepoMock.Setup(repo => repo.ViewProfileUser(It.IsAny<Guid>(), default))
					 .ThrowsAsync(new Exception());

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.ViewProfile(_user.Id));
		_cacheServiceMock.Verify(cache => cache.Get<User>(It.IsAny<string>()), Times.Once);
		_userRepoMock.Verify(repo => repo.ViewProfileUser(It.IsAny<Guid>(), default), Times.Once);
		_cacheServiceMock.Verify(cache => cache.Set(It.IsAny<string>(), It.IsAny<User>(), It.IsAny<TimeSpan>()), Times.Never);
	}

	[Fact]
	public async Task ViewProfileAsync_Should_ThrowException_WhenCacheGetFails()
	{
		// Arrange
		_cacheServiceMock.Setup(cache => cache.Get<User>(It.IsAny<string>()))
						 .ThrowsAsync(new Exception());

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.ViewProfile(_user.Id));
		_cacheServiceMock.Verify(cache => cache.Get<User>(It.IsAny<string>()), Times.Once);
		_userRepoMock.Verify(repo => repo.ViewProfileUser(It.IsAny<Guid>(), default), Times.Never);
		_cacheServiceMock.Verify(cache => cache.Set(It.IsAny<string>(), It.IsAny<User>(), It.IsAny<TimeSpan>()), Times.Never);
	}

	[Fact]
	public async Task ViewProfile_Should_ThrowException_WhenCacheSetFails()
	{
		// Arrange
		_cacheServiceMock.Setup(cache => cache.Get<User>(It.IsAny<string>()))
						 .ReturnsAsync((User)null);
		_userRepoMock.Setup(repo => repo.ViewProfileUser(It.IsAny<Guid>(), default))
					 .ReturnsAsync(_user);
		_cacheServiceMock.Setup(cache => cache.Set(It.IsAny<string>(), It.IsAny<User>(), TimeSpan.FromMinutes(10)))
						 .ThrowsAsync(new Exception());

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.ViewProfile(_user.Id));
		_cacheServiceMock.Verify(cache => cache.Get<User>(It.IsAny<string>()), Times.Once);
		_userRepoMock.Verify(repo => repo.ViewProfileUser(It.IsAny<Guid>(), default), Times.Once);
		_cacheServiceMock.Verify(cache => cache.Set(It.IsAny<string>(), It.IsAny<User>(), It.IsAny<TimeSpan>()), Times.Once);
	}
}
