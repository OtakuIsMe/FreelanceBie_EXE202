using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Notifications;
public class ViewNotificationsServiceTests
{
	private readonly Mock<INotificationRepo> _notificationRepoMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly NotificationServ _notificationServ;
	private readonly Guid _userId = Guid.NewGuid();
	public ViewNotificationsServiceTests()
	{
		_notificationRepoMock = new Mock<INotificationRepo>();
		_cacheServiceMock = new Mock<ICacheService>();
		_notificationServ = new NotificationServ(
			_notificationRepoMock.Object,
			_cacheServiceMock.Object);
	}

	[Fact]
	public async Task ViewNotificationsAsync_Should_ReturnSuccess_WhenCachedNotificationsIfExists()
	{
		// Arrange
		var cachedNotifications = new List<Notification>
		{
			new Notification { Id = Guid.NewGuid(), UserId = _userId, Message = "Cached Notification" }
		};

		_cacheServiceMock.Setup(c => c.Get<List<Notification>>(It.IsAny<string>()))
						 .ReturnsAsync(cachedNotifications);

		// Act
		var result = await _notificationServ.ViewNotifications(_userId);
		var jsonResult = Assert.IsType<JsonResult>(result);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.Equal(cachedNotifications, jsonResult.Value);
		_cacheServiceMock.Verify(c => c.Get<List<Notification>>(It.IsAny<string>()), Times.Once);
		_notificationRepoMock.Verify(n => n.GetNotificationsByUserId(It.IsAny<Guid>()), Times.Never);
		_cacheServiceMock.Verify(c =>
			c.Set(It.IsAny<string>(), It.IsAny<List<Notification>>(), It.IsAny<TimeSpan>()), Times.Never);
	}

	[Fact]
	public async Task ViewNotificationsAsync_Should_ReturnSuccess_WhenFetchingFromRepoIfCacheMisses()
	{
		// Arrange
		var notifications = new List<Notification>
		{
			new Notification { Id = Guid.NewGuid(), UserId = _userId, Message = "New Notification" }
		};

		_cacheServiceMock.Setup(c => c.Get<List<Notification>>(It.IsAny<string>()))
			.ReturnsAsync((List<Notification>)null);
		_notificationRepoMock.Setup(n => n.GetNotificationsByUserId(It.IsAny<Guid>()))
			.ReturnsAsync(notifications);
		_cacheServiceMock.Setup(cache => cache.Set(It.IsAny<string>(), notifications, TimeSpan.FromMinutes(5)))
			.Returns(Task.CompletedTask);

		// Act
		var result = await _notificationServ.ViewNotifications(_userId);
		var jsonResult = Assert.IsType<JsonResult>(result);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.Equal(notifications, jsonResult.Value);
		_cacheServiceMock.Verify(c => c.Get<List<Notification>>(It.IsAny<string>()), Times.Once);
		_notificationRepoMock.Verify(n => n.GetNotificationsByUserId(It.IsAny<Guid>()), Times.Once);
		_cacheServiceMock.Verify(c =>
			c.Set(It.IsAny<string>(), It.IsAny<List<Notification>>(), It.IsAny<TimeSpan>()), Times.Once);
	}

	[Fact]
	public async Task ViewNotificationsAsync_Should_ThrowException_IfNoNotificationsFound()
	{
		// Arrange
		_cacheServiceMock.Setup(c => c.Get<List<Notification>>($"notifications-{_userId}"))
						 .ReturnsAsync((List<Notification>)null);
		_notificationRepoMock.Setup(n => n.GetNotificationsByUserId(_userId))
							 .ReturnsAsync(new List<Notification>());

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _notificationServ.ViewNotifications(_userId));
		_cacheServiceMock.Verify(c => c.Get<List<Notification>>(It.IsAny<string>()), Times.Once);
		_notificationRepoMock.Verify(n => n.GetNotificationsByUserId(It.IsAny<Guid>()), Times.Once);
		_cacheServiceMock.Verify(c =>
			c.Set(It.IsAny<string>(), It.IsAny<List<Notification>>(), It.IsAny<TimeSpan>()), Times.Never);
	}

	[Fact]
	public async Task ViewNotificationsAsync_Should_ThrowException_WhenHandlingRepositoryException()
	{
		// Arrange
		_cacheServiceMock.Setup(c => c.Get<List<Notification>>(It.IsAny<string>()))
						 .ReturnsAsync((List<Notification>)null);
		_notificationRepoMock.Setup(n => n.GetNotificationsByUserId(It.IsAny<Guid>()))
							 .ThrowsAsync(new Exception());

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _notificationServ.ViewNotifications(_userId));
		_cacheServiceMock.Verify(c => c.Get<List<Notification>>(It.IsAny<string>()), Times.Once);
		_notificationRepoMock.Verify(n => n.GetNotificationsByUserId(It.IsAny<Guid>()), Times.Once);
		_cacheServiceMock.Verify(c =>
			c.Set(It.IsAny<string>(), It.IsAny<List<Notification>>(), It.IsAny<TimeSpan>()), Times.Never);
	}
}
