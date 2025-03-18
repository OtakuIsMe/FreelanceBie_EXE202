using BE.src.api.domains.Enum;
using BE.src.api.domains.eventbus.Producers;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Service.UnitTests.Users;
public class GetAllUsersServiceTests
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
	public GetAllUsersServiceTests()
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
	public async Task GetAllUsersAsync_Should_ReturnSuccess_WhenCacheExists()
	{
		// Arrange
		var cachedUsers = new List<User>
		{
			new User
			{
				Id = Guid.NewGuid(),
				Name = "John Doe",
				Email = "John@example.com",
				Password = "123",
				Role = RoleEnum.Customer,
				Username = "John"
			}
		};
		_cacheServiceMock.Setup(c => c.Get<List<User>>(It.IsAny<string>()))
			.ReturnsAsync(cachedUsers);

		// Act
		var result = await _userServ.GetAllUsers();
		var jsonResult = Assert.IsType<JsonResult>(result);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.Equal(cachedUsers, jsonResult.Value);
		_cacheServiceMock.Verify(c => c.Get<List<User>>(It.IsAny<string>()), Times.Once);
		_userRepoMock.Verify(repo => repo.GetUsers(), Times.Never);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), cachedUsers, TimeSpan.FromMinutes(10)), Times.Never);
	}

	[Fact]
	public async Task GetAllUsersAsync_Should_ReturnSuccess_WhenFetchedFromRepository()
	{
		// Arrange
		var users = new List<User>
		{
			new User
			{
				Id = Guid.NewGuid(),
				Name = "Jane Doe",
				Email = "Jane@example.com",
				Password = "123",
				Role = RoleEnum.Customer,
				Username = "Jane"
			}
		};
		_cacheServiceMock.Setup(c => c.Get<List<User>>(It.IsAny<string>()))
			.ReturnsAsync((List<User>)null);
		_userRepoMock.Setup(r => r.GetUsers())
			.ReturnsAsync(users);
		_cacheServiceMock.Setup(c => c.Set(It.IsAny<string>(), users, TimeSpan.FromMinutes(10)))
			.Returns(Task.CompletedTask);

		// Act
		var result = await _userServ.GetAllUsers();
		var jsonResult = Assert.IsType<JsonResult>(result);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.Equal(users, jsonResult.Value);
		_cacheServiceMock.Verify(c => c.Get<List<User>>(It.IsAny<string>()), Times.Once);
		_userRepoMock.Verify(repo => repo.GetUsers(), Times.Once);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), users, TimeSpan.FromMinutes(10)), Times.Once);
	}

	[Fact]
	public async Task GetAllUsersAsync_Should_ThrowException_WhenNoUsersFound()
	{
		// Arrange
		var users = new List<User> { };
		_cacheServiceMock.Setup(c => c.Get<List<User>>(It.IsAny<string>()))
			.ReturnsAsync((List<User>)null);
		_userRepoMock.Setup(r => r.GetUsers())
			.ReturnsAsync(new List<User>());

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.GetAllUsers());
		_cacheServiceMock.Verify(c => c.Get<List<User>>(It.IsAny<string>()), Times.Once);
		_userRepoMock.Verify(repo => repo.GetUsers(), Times.Once);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), users, TimeSpan.FromMinutes(10)), Times.Never);
	}

	[Fact]
	public async Task GetAllUsersAsync_Should_ThrowException_WhenRepositoryThrowsException()
	{
		// Arrange
		var users = new List<User> { };
		_cacheServiceMock.Setup(c => c.Get<List<User>>(It.IsAny<string>()))
			.ReturnsAsync((List<User>)null);
		_userRepoMock.Setup(r => r.GetUsers())
			.ThrowsAsync(new Exception());

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.GetAllUsers());
		_cacheServiceMock.Verify(c => c.Get<List<User>>(It.IsAny<string>()), Times.Once);
		_userRepoMock.Verify(repo => repo.GetUsers(), Times.Once);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), users, TimeSpan.FromMinutes(10)), Times.Never);
	}
}
