using BE.src.api.domains.DTOs.User;
using BE.src.api.domains.eventbus.Producers;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Service.UnitTests.Users;
public class AddCommentServiceTests
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
	private readonly Guid _shotId = Guid.NewGuid();
	private readonly UserServ _userServ;
	public AddCommentServiceTests()
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
	public async Task AddCommentAsync_Should_ReturnSuccess_WhenCommentIsAddedSuccessfully()
	{
		// Arrange
		var commentData = new AddCommentDTO
		{
			ShotId = _shotId,
			Description = "This is a test comment"
		};

		_userRepoMock.Setup(repo => repo.AddComment(It.IsAny<Comment>())).ReturnsAsync(true);

		// Act
		var result = await _userServ.AddComment(_userId, commentData);
		var jsonResult = Assert.IsType<JsonResult>(result);

		// Assert
		Assert.Equal(201, jsonResult.StatusCode);
		_userRepoMock.Verify(repo => repo.AddComment(It.IsAny<Comment>()), Times.Once);
	}

	[Fact]
	public async Task AddCommentAsync_Should_ThrowException_WhenCommentCreationFails()
	{
		// Arrange
		var commentData = new AddCommentDTO
		{
			ShotId = _shotId,
			Description = "This is a failed comment"
		};

		_userRepoMock.Setup(repo => repo.AddComment(It.IsAny<Comment>())).ReturnsAsync(false);

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.AddComment(_userId, commentData));
		_userRepoMock.Verify(repo => repo.AddComment(It.IsAny<Comment>()), Times.Once);
	}

	[Fact]
	public async Task AddCommentAsync_Should_ThrowException_WhenExceptionOccurs()
	{
		// Arrange
		var commentData = new AddCommentDTO
		{
			ShotId = _shotId,
			Description = "This is a test comment"
		};

		_userRepoMock.Setup(repo => repo.AddComment(It.IsAny<Comment>())).ThrowsAsync(new Exception());

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.AddComment(_userId, commentData));
		_userRepoMock.Verify(repo => repo.AddComment(It.IsAny<Comment>()), Times.Once);
	}
}
