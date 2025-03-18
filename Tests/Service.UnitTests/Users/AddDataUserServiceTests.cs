using BE.src.api.domains.DTOs.User;
using BE.src.api.domains.Enum;
using BE.src.api.domains.eventbus.Producers;
using BE.src.api.domains.Model;
using BE.src.api.helpers;
using BE.src.api.repositories;
using BE.src.api.services;
using DotNetEnv;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Service.UnitTests.Users;
public class AddDataUserServiceTests
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
	public AddDataUserServiceTests()
	{
		var envPath = Path.GetFullPath(Path.Combine(
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
	public async Task AddDataUserAsync_Should_ReturnSuccess_WhenUserIsAddedSuccessfully()
	{
		// Arrange
		var userData = new UserAddData
		{
			Name = "Test User",
			Username = "testuser",
			Password = "123",
			Email = "test@example.com",
			Role = RoleEnum.Customer
		};
		_userRepoMock.Setup(repo => repo.CreateUser(It.IsAny<User>())).ReturnsAsync(true);

		// Act
		var result = await _userServ.AddDataUser(userData);
		var jsonResult = Assert.IsType<JsonResult>(result);

		// Assert
		Assert.Equal(201, jsonResult.StatusCode);
		_userRepoMock.Verify(repo => repo.CreateUser(It.IsAny<User>()), Times.Once);
	}

	[Fact]
	public async Task AddDataUserAsync_Should_ThrowException_WhenUserCreationFails()
	{
		// Arrange
		var userData = new UserAddData
		{
			Name = "Test User",
			Username = "testuser",
			Password = "123",
			Email = "test@example.com",
			Role = RoleEnum.Customer
		};
		_userRepoMock.Setup(repo => repo.CreateUser(It.IsAny<User>())).ReturnsAsync(false);

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(async () => await _userServ.AddDataUser(userData));
		_userRepoMock.Verify(repo => repo.CreateUser(It.IsAny<User>()), Times.Once);
	}

	[Fact]
	public async Task AddDataUserAsync_Should_ReturnSuccess_WhenAvatarIsProvided()
	{
		// Arrange
		var avatarStream = new MemoryStream();
		var writer = new StreamWriter(avatarStream);
		writer.Write("fake image data");
		writer.Flush();
		avatarStream.Position = 0;

		var formFile = new FormFile(avatarStream, 0, avatarStream.Length, "avatar", "avatar.png")
		{
			Headers = new HeaderDictionary(),
			ContentType = "image/png"
		};

		var userData = new UserAddData
		{
			Name = "Test User",
			Username = "testuser",
			Password = "123",
			Email = "test@example.com",
			Role = RoleEnum.Customer,
			Avatar = formFile
		};

		_userRepoMock.Setup(repo => repo.CreateUser(It.IsAny<User>())).ReturnsAsync(true);

		// Act
		var result = await _userServ.AddDataUser(userData);
		var jsonResult = Assert.IsType<JsonResult>(result);

		// Assert
		Assert.Equal(201, jsonResult.StatusCode);
		_userRepoMock.Verify(repo => repo.CreateUser(It.Is<User>(u => u.ImageVideos.Any(img => img.Url.Contains("https://")))), Times.Once);
	}

	[Fact]
	public async Task AddDataUserAsync_Should_ReturnSuccess_WhenBackgroundIsProvided()
	{
		// Arrange
		var avatarStream = new MemoryStream();
		var writer = new StreamWriter(avatarStream);
		writer.Write("fake image data");
		writer.Flush();
		avatarStream.Position = 0;

		var formFile = new FormFile(avatarStream, 0, avatarStream.Length, "background", "background.png")
		{
			Headers = new HeaderDictionary(),
			ContentType = "image/png"
		};

		var userData = new UserAddData
		{
			Name = "Test User",
			Username = "testuser",
			Password = "123",
			Email = "test@example.com",
			Role = RoleEnum.Customer,
			Background = formFile
		};
		_userRepoMock.Setup(repo => repo.CreateUser(It.IsAny<User>())).ReturnsAsync(true);

		// Act
		var result = await _userServ.AddDataUser(userData);
		var jsonResult = Assert.IsType<JsonResult>(result);

		// Assert
		Assert.Equal(201, jsonResult.StatusCode);
		_userRepoMock.Verify(repo => repo.CreateUser(It.Is<User>(u => u.ImageVideos.Any(img => img.Url.Contains("https://")))), Times.Once);
	}
}
