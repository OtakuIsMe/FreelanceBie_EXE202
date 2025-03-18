using System.Text.Json;
using BE.src.api.domains.eventbus.Producers;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;

namespace Service.UnitTests.Users;
public class FollowChangeServiceTests
{
	private readonly Mock<IUserRepo> _userRepoMock;
	private readonly Mock<IEmailServ> _emailServMock;
	private readonly Mock<ISocialProfileRepo> _socialProfileRepoMock;
	private readonly Mock<IMembershipRepo> _membershipRepoMock;
	private readonly Mock<IPostRepo> _postRepoMock;
	private readonly Mock<INotificationRepo> _notificationRepoMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly Mock<IEventBusRabbitMQProducer> _eventBusRabbitMQProducerMock;
	private readonly Guid _userId = Guid.NewGuid();
	private readonly Guid _followedId = Guid.NewGuid();
	private readonly UserServ _userServ;
	public FollowChangeServiceTests()
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
	public async Task FollowChangeAsync_Should_ReturnSuccess_WhenAlreadyFollowing()
	{
		// Arrange
		_cacheServiceMock.Setup(cache => cache.Get<bool?>(It.IsAny<string>())).ReturnsAsync(true);

		// Act
		var result = await _userServ.FollowChange(_userId, _followedId, true);
		var jsonResult = Assert.IsType<JsonResult>(result);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		_cacheServiceMock.Verify(cache => cache.Get<bool?>(It.IsAny<string>()), Times.Once);
		_userRepoMock.Verify(repo => repo.GetFollow(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
		_userRepoMock.Verify(repo => repo.CreateFollow(It.IsAny<Follow>()), Times.Never);
		_cacheServiceMock.Verify(cache => cache.Set(It.IsAny<string>(), true, TimeSpan.FromDays(15)), Times.Never);
		_userRepoMock.Verify(repo => repo.DeleteFollow(It.IsAny<Follow>()), Times.Never);
		_cacheServiceMock.Verify(cache => cache.Remove(It.IsAny<string>()), Times.Never);
	}

	[Fact]
	public async Task FollowChangeAsync_Should_ReturnSuccess_WhenFollowingSuccess()
	{
		// Arrange
		_cacheServiceMock.Setup(cache => cache.Get<bool?>(It.IsAny<string>())).ReturnsAsync((bool?)null);
		_userRepoMock.Setup(repo => repo.GetFollow(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync((Follow)null);
		_userRepoMock.Setup(repo => repo.CreateFollow(It.IsAny<Follow>()))
			.ReturnsAsync(true);
		_cacheServiceMock.Setup(cache => cache.Set(It.IsAny<string>(), true, TimeSpan.FromDays(15)))
			.Returns(Task.CompletedTask);

		// Act
		var result = await _userServ.FollowChange(_userId, _followedId, true);
		var jsonResult = Assert.IsType<JsonResult>(result);

		// Assert
		Assert.Equal(201, jsonResult.StatusCode);
		_cacheServiceMock.Verify(cache => cache.Get<bool?>(It.IsAny<string>()), Times.Once);
		_userRepoMock.Verify(repo => repo.GetFollow(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
		_userRepoMock.Verify(repo => repo.CreateFollow(It.IsAny<Follow>()), Times.Once);
		_cacheServiceMock.Verify(cache => cache.Set(It.IsAny<string>(), true, TimeSpan.FromDays(15)), Times.Once);
		_userRepoMock.Verify(repo => repo.DeleteFollow(It.IsAny<Follow>()), Times.Never);
		_cacheServiceMock.Verify(cache => cache.Remove(It.IsAny<string>()), Times.Never);
	}

	[Fact]
	public async Task FollowChangeAsync_Should_ReturnSuccess_WhenAlreadyUnfollowed()
	{
		// Arrange
		_cacheServiceMock.Setup(cache => cache.Get<bool?>(It.IsAny<string>())).ReturnsAsync(false);

		// Act
		var result = await _userServ.FollowChange(_userId, _followedId, false);
		var jsonResult = Assert.IsType<JsonResult>(result);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		_cacheServiceMock.Verify(cache => cache.Get<bool?>(It.IsAny<string>()), Times.Once);
		_userRepoMock.Verify(repo => repo.GetFollow(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
		_userRepoMock.Verify(repo => repo.CreateFollow(It.IsAny<Follow>()), Times.Never);
		_cacheServiceMock.Verify(cache => cache.Set(It.IsAny<string>(), true, TimeSpan.FromDays(15)), Times.Never);
		_userRepoMock.Verify(repo => repo.DeleteFollow(It.IsAny<Follow>()), Times.Never);
		_cacheServiceMock.Verify(cache => cache.Remove(It.IsAny<string>()), Times.Never);
	}

	[Fact]
	public async Task FollowChangeAsync_Should_ReturnSuccess_WhenUnfollowSuccess()
	{
		// Arrange
		var existingFollow = new Follow { FollowedId = _followedId, FollowingId = _userId };

		_cacheServiceMock.Setup(cache => cache.Get<bool?>(It.IsAny<string>()))
			.ReturnsAsync((bool?)null);
		_userRepoMock.Setup(repo => repo.GetFollow(It.IsAny<Guid>(), It.IsAny<Guid>()))
			.ReturnsAsync(existingFollow);
		_userRepoMock.Setup(repo => repo.DeleteFollow(It.IsAny<Follow>()))
			.ReturnsAsync(true);
		_cacheServiceMock.Setup(cache => cache.Remove(It.IsAny<string>()))
			.Returns(Task.CompletedTask);

		// Act
		var result = await _userServ.FollowChange(_userId, _followedId, false);
		var jsonResult = Assert.IsType<JsonResult>(result);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		_cacheServiceMock.Verify(cache => cache.Get<bool?>(It.IsAny<string>()), Times.Once);
		_userRepoMock.Verify(repo => repo.GetFollow(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
		_userRepoMock.Verify(repo => repo.CreateFollow(It.IsAny<Follow>()), Times.Never);
		_cacheServiceMock.Verify(cache => cache.Set(It.IsAny<string>(), true, TimeSpan.FromDays(15)), Times.Never);
		_userRepoMock.Verify(repo => repo.DeleteFollow(It.IsAny<Follow>()), Times.Once);
		_cacheServiceMock.Verify(cache => cache.Remove(It.IsAny<string>()), Times.Once);
	}

	[Fact]
	public async Task FollowChangeAsync_Should_ThrowException_WhenExceptionOccurs()
	{
		// Arrange
		_cacheServiceMock.Setup(cache => cache.Get<bool?>(It.IsAny<string>()))
			.ReturnsAsync((bool?)null);
		_userRepoMock.Setup(repo => repo.GetFollow(It.IsAny<Guid>(), It.IsAny<Guid>()))
			.ThrowsAsync(new Exception());

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.FollowChange(_userId, _followedId, true));
		_cacheServiceMock.Verify(cache => cache.Get<bool?>(It.IsAny<string>()), Times.Once);
		_userRepoMock.Verify(repo => repo.GetFollow(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
		_userRepoMock.Verify(repo => repo.CreateFollow(It.IsAny<Follow>()), Times.Never);
		_cacheServiceMock.Verify(cache => cache.Set(It.IsAny<string>(), true, TimeSpan.FromDays(15)), Times.Never);
		_userRepoMock.Verify(repo => repo.DeleteFollow(It.IsAny<Follow>()), Times.Never);
		_cacheServiceMock.Verify(cache => cache.Remove(It.IsAny<string>()), Times.Never);
	}
}
