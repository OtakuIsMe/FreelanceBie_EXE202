using System.Text.Json;
using BE.src.api.domains.DTOs.User;
using BE.src.api.domains.Enum;
using BE.src.api.domains.eventbus.Producers;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Users;
public class SearchingDesignersServiceTests
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
	public SearchingDesignersServiceTests()
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

	private List<User> CreateFakeUser()
	{
		return new List<User>
		{
			new User
			{
				Id = Guid.NewGuid(),
				Name = "User",
				Phone = "987654321",
				City = "Old City",
				Email = "user@example.com",
				Password = "123",
				Role = RoleEnum.Customer,
				Username = "User1"
			}
		};
	}

	[Fact]
	public async Task SearchingDesignersAsync_Should_ReturnSuccess_WhenCacheExists()
	{
		// Arrange
		var userSearchingDto = new UserSearchingDTO { Name = "User" };
		var cachedUsers = CreateFakeUser();

		_cacheServiceMock.Setup(cache => cache.Get<List<User>>(It.IsAny<string>()))
						 .ReturnsAsync(cachedUsers);

		// Act
		var result = await _userServ.SearchingDesigners(userSearchingDto);
		var jsonResult = Assert.IsType<JsonResult>(result);
		var json = JsonSerializer.Serialize(jsonResult.Value);
		var response = JsonSerializer.Deserialize<List<User>>(json);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.NotNull(response);
		Assert.Single(response);
		Assert.Equal("User", response[0].Name);
		_cacheServiceMock.Verify(cache => cache.Get<List<User>>(It.IsAny<string>()), Times.Once);
		_userRepoMock.Verify(repo => repo.FindUsers(It.IsAny<UserSearchingDTO>()), Times.Never);
		_cacheServiceMock.Verify(cache => cache.Set(It.IsAny<string>(), cachedUsers, TimeSpan.FromMinutes(10)), Times.Never);
	}

	[Fact]
	public async Task SearchingDesignersAsync_Should_ReturnSuccess_WhenGettingFromRepoIfCacheIsEmpty()
	{
		// Arrange
		var userSearchingDto = new UserSearchingDTO { Name = "User" };
		var usersFromRepo = CreateFakeUser();

		_cacheServiceMock.Setup(cache => cache.Get<List<User>>(It.IsAny<string>()))
						 .ReturnsAsync((List<User>)null);
		_userRepoMock.Setup(repo => repo.FindUsers(It.IsAny<UserSearchingDTO>()))
					 .ReturnsAsync(usersFromRepo);
		_cacheServiceMock.Setup(cache => cache.Set(It.IsAny<string>(), It.IsAny<List<User>>(), It.IsAny<TimeSpan>()))
						 .Returns(Task.CompletedTask);

		// Act
		var result = await _userServ.SearchingDesigners(userSearchingDto);
		var jsonResult = Assert.IsType<JsonResult>(result);
		var json = JsonSerializer.Serialize(jsonResult.Value);
		var response = JsonSerializer.Deserialize<List<User>>(json);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.NotNull(response);
		Assert.Single(response);
		Assert.Equal("User", response[0].Name);

		_cacheServiceMock.Verify(cache => cache.Get<List<User>>(It.IsAny<string>()), Times.Once);
		_userRepoMock.Verify(repo => repo.FindUsers(It.IsAny<UserSearchingDTO>()), Times.Once);
		_cacheServiceMock.Verify(cache => cache.Set(It.IsAny<string>(), It.IsAny<List<User>>(), It.IsAny<TimeSpan>()), Times.Once);
	}

	[Fact]
	public async Task SearchingDesignersAsync_Should_ThrowException_WhenNoUsersFound()
	{
		// Arrange
		var userSearchingDto = new UserSearchingDTO { Name = "Unknown" };

		_cacheServiceMock.Setup(cache => cache.Get<List<User>>(It.IsAny<string>()))
						 .ReturnsAsync((List<User>)null);
		_userRepoMock.Setup(repo => repo.FindUsers(It.IsAny<UserSearchingDTO>()))
					 .ReturnsAsync(new List<User>());

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.SearchingDesigners(userSearchingDto));
		_cacheServiceMock.Verify(cache => cache.Get<List<User>>(It.IsAny<string>()), Times.Once);
		_userRepoMock.Verify(repo => repo.FindUsers(It.IsAny<UserSearchingDTO>()), Times.Once);
		_cacheServiceMock.Verify(cache => cache.Set(It.IsAny<string>(), It.IsAny<List<User>>(), It.IsAny<TimeSpan>()), Times.Never);
	}

	[Fact]
	public async Task SearchingDesignersAsync_Should_ThrowException_WhenGettingFromRepositoryFails()
	{
		// Arrange
		var userSearchingDto = new UserSearchingDTO { Name = "ErrorCase" };

		_cacheServiceMock.Setup(cache => cache.Get<List<User>>(It.IsAny<string>()))
						 .ReturnsAsync((List<User>)null);
		_userRepoMock.Setup(repo => repo.FindUsers(It.IsAny<UserSearchingDTO>()))
					 .ThrowsAsync(new Exception());

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.SearchingDesigners(userSearchingDto));

		_cacheServiceMock.Verify(cache => cache.Get<List<User>>(It.IsAny<string>()), Times.Once);
		_userRepoMock.Verify(repo => repo.FindUsers(It.IsAny<UserSearchingDTO>()), Times.Once);
		_cacheServiceMock.Verify(cache => cache.Set(It.IsAny<string>(), It.IsAny<List<User>>(), It.IsAny<TimeSpan>()), Times.Never);
	}
}
