using BE.src.api.domains.Enum;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.PostJobs;
public class ApplyJobServiceTests
{
	private readonly PostServ _postServ;
	private readonly Mock<IPostRepo> _postRepoMock;
	private readonly Mock<ICacheService> _cachServMock;
	private readonly Mock<INotificationRepo> _notificationRepoMock;
	private readonly Mock<ISpecialtyRepo> _specialtyRepoMock;
	private readonly Guid _userId = Guid.NewGuid();
	private readonly Guid _postId = Guid.NewGuid();
	public ApplyJobServiceTests()
	{
		string envPath = Path.GetFullPath(Path.Combine
			(AppDomain.CurrentDomain.BaseDirectory, "../../../../../BE/.env"));

		Env.Load(envPath);

		_postRepoMock = new Mock<IPostRepo>();
		_notificationRepoMock = new Mock<INotificationRepo>();
		_cachServMock = new Mock<ICacheService>();
		_specialtyRepoMock = new Mock<ISpecialtyRepo>();

		_postServ = new PostServ(
			_postRepoMock.Object,
			_cachServMock.Object,
			_notificationRepoMock.Object,
			_specialtyRepoMock.Object);
	}

	[Fact]
	public async Task ApplyJobAsync_Should_ReturnSuccess_WhenApplyJobIfNotificationDoesNotExist()
	{
		// Arrange
		var post = new PostJob
		{
			UserId = _userId,
			Id = _postId,
			Title = "Test Post",
			Description = "This is a test description",
			SpecialtyId = Guid.NewGuid(),
			WorkType = WorkTypeEnum.Remote,
			WorkLocation = "USA",
			CompanyName = "TestCorp",
			EmploymentType = EmploymentTypeEnum.FullTime,
			Experience = 3,
			CompanyLink = "https://company.com",
			Payment = 5000,
		};

		_notificationRepoMock.Setup(n => n.GetNotificationByPostId(_postId))
			.ReturnsAsync((Notification)null);
		_postRepoMock.Setup(p => p.GetPostJobById(_postId))
			.ReturnsAsync(post);
		_notificationRepoMock.Setup(n => n.CreateNotification(It.IsAny<Notification>()))
			.ReturnsAsync(true);
		_postRepoMock.Setup(p => p.CreateUserApply(It.IsAny<UserApply>()))
			.ReturnsAsync(true);

		// Act
		var result = await _postServ.ApplyJob(_userId, _postId);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.Equal(201, jsonResult.StatusCode);
		_notificationRepoMock.Verify(n => n.GetNotificationByPostId(_postId), Times.Once);
		_postRepoMock.Verify(p => p.GetPostJobById(_postId), Times.Once);
		_notificationRepoMock.Verify(n => n.CreateNotification(It.IsAny<Notification>()), Times.Once);
		_postRepoMock.Verify(p => p.CreateUserApply(It.IsAny<UserApply>()), Times.Once);
	}

	[Fact]
	public async Task ApplyJobAsync_Should_ReturnSuccess_WhenApplyJobIfNotificationExists()
	{
		// Arrange
		var notification = new Notification { Id = Guid.NewGuid(), UserId = _userId, Message = "Apply job", CountUser = 1};

		_notificationRepoMock.Setup(n => n.GetNotificationByPostId(_postId))
			.ReturnsAsync(notification);
		_notificationRepoMock.Setup(n => n.UpdateNotification(It.IsAny<Notification>()))
			.ReturnsAsync(true);
		_postRepoMock.Setup(p => p.CreateUserApply(It.IsAny<UserApply>()))
			.ReturnsAsync(true);

		// Act
		var result = await _postServ.ApplyJob(_userId, _postId);

		// Assert
		Assert.Equal(2, ++notification.CountUser);

		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.Equal(201, jsonResult.StatusCode);
		_notificationRepoMock.Verify(n => n.GetNotificationByPostId(_postId), Times.Once);
		_notificationRepoMock.Verify(n => n.UpdateNotification(It.IsAny<Notification>()), Times.Once);
		_postRepoMock.Verify(p => p.CreateUserApply(It.IsAny<UserApply>()), Times.Once);
	}

	[Fact]
	public async Task ApplyJobAsync_Should_ThrowException_WhenPostNotFound()
	{
		// Arrange
		_notificationRepoMock.Setup(n => n.GetNotificationByPostId(_postId))
			.ReturnsAsync((Notification)null);
		_postRepoMock.Setup(repo => repo.GetPostJobById(_postId))
			.ReturnsAsync((PostJob)null);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _postServ.ApplyJob(_userId, _postId));
		_notificationRepoMock.Verify(n => n.GetNotificationByPostId(_postId), Times.Once);
		_postRepoMock.Verify(p => p.GetPostJobById(_postId), Times.Once);
		_postRepoMock.Verify(p => p.CreateUserApply(It.IsAny<UserApply>()), Times.Never);
	}

	[Fact]
	public async Task ApplyJobAsync_Should_ThrowException_WhenCreateNotificationFails()
	{
		// Arrange
		var post = new PostJob
		{
			UserId = _userId,
			Id = _postId,
			Title = "Test Post",
			Description = "This is a test description",
			SpecialtyId = Guid.NewGuid(),
			WorkType = WorkTypeEnum.Remote,
			WorkLocation = "USA",
			CompanyName = "TestCorp",
			EmploymentType = EmploymentTypeEnum.FullTime,
			Experience = 3,
			CompanyLink = "https://company.com",
			Payment = 5000,
		};

		_notificationRepoMock.Setup(repo => repo.GetNotificationByPostId(_postId))
			.ReturnsAsync((Notification)null);
		_postRepoMock.Setup(repo => repo.GetPostJobById(_postId))
			.ReturnsAsync(post);
		_notificationRepoMock.Setup(repo => repo.CreateNotification(It.IsAny<Notification>()))
			.Throws(new Exception("Database error"));

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _postServ.ApplyJob(_userId, _postId));
		_notificationRepoMock.Verify(n => n.GetNotificationByPostId(_postId), Times.Once);
		_postRepoMock.Verify(p => p.GetPostJobById(_postId), Times.Once);
		_notificationRepoMock.Verify(n => n.CreateNotification(It.IsAny<Notification>()), Times.Once);
		_postRepoMock.Verify(p => p.CreateUserApply(It.IsAny<UserApply>()), Times.Never);
	}

	[Fact]
	public async Task ApplyJobAsync_Should_ThrowException_WhenUpdateNotificationFails()
	{
		// Arrange
		var notification = new Notification { Id = Guid.NewGuid(), UserId = _userId, Message = "Apply job", CountUser = 1 };

		_notificationRepoMock.Setup(repo => repo.GetNotificationByPostId(_postId))
			.ReturnsAsync(notification);
		_notificationRepoMock.Setup(repo => repo.UpdateNotification(It.IsAny<Notification>()))
			.Throws(new Exception("Database error"));

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _postServ.ApplyJob(_userId, _postId));
		_notificationRepoMock.Verify(n => n.GetNotificationByPostId(_postId), Times.Once);
		_notificationRepoMock.Verify(n => n.UpdateNotification(It.IsAny<Notification>()), Times.Once);
		_postRepoMock.Verify(p => p.CreateUserApply(It.IsAny<UserApply>()), Times.Never);
	}

	[Fact]
	public async Task ApplyJobAsync_Should_ThrowException_WhenCreateUserApplyFails()
	{
		// Arrange
		var notification = new Notification { Id = Guid.NewGuid(), UserId = _userId, Message = "Apply job", CountUser = 1 };

		_notificationRepoMock.Setup(repo => repo.GetNotificationByPostId(_postId))
			.ReturnsAsync(notification);
		_notificationRepoMock.Setup(repo => repo.UpdateNotification(It.IsAny<Notification>()))
			.ReturnsAsync(true);
		_postRepoMock.Setup(repo => repo.CreateUserApply(It.IsAny<UserApply>()))
			.Throws(new Exception("Database Errors"));
		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _postServ.ApplyJob(_userId, _postId));

		_notificationRepoMock.Verify(n => n.GetNotificationByPostId(_postId), Times.Once);
		_notificationRepoMock.Verify(n => n.UpdateNotification(It.IsAny<Notification>()), Times.Once);
		_postRepoMock.Verify(p => p.CreateUserApply(It.IsAny<UserApply>()), Times.Once);
	}
}
