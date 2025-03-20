using System.Collections;
using BE.src.api.domains.DTOs.Post;
using BE.src.api.domains.Enum;
using BE.src.api.domains.eventbus.Producers;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using BE.src.api.shared.Constant;
using DotNetEnv;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Users;
public class CheckNotificationWhenPostServiceTests
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
	private readonly Guid _postId = Guid.NewGuid();
	private readonly Guid _membershipId = Guid.NewGuid();
	private readonly UserServ _userServ;
	public CheckNotificationWhenPostServiceTests()
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

	private PostJob createFakePostJob()
	{
		return new PostJob
		{
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
			Payment = 5000
		};
	}

	[Fact]
	public async Task CheckNotificationWhenPostAsync_Should_ReturnSuccess_IfDataExistsInCache()
	{
		// Arrange
		var cachedPost = createFakePostJob();

		_cacheServiceMock.Setup(c => c.Get<string>(It.IsAny<string>()))
						 .ReturnsAsync(_membershipId.ToString());
		_cacheServiceMock.Setup(c => c.Get<bool?>(It.IsAny<string>()))
						 .ReturnsAsync(true);
		_cacheServiceMock.Setup(c => c.Get<PostJob>(It.IsAny<string>()))
						 .ReturnsAsync(cachedPost);
		_eventBusRabbitMQProducerMock.Setup(rbmq =>
			rbmq.Publish(EventBus.PostNotificationQueue, It.IsAny<PostCreatedEvent>()));

		// Act
		var result = await _userServ.CheckNotificationWhenPost(_userId);
		var jsonResult = Assert.IsType<JsonResult>(result);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		_cacheServiceMock.Verify(c => c.Get<string>(It.IsAny<string>()), Times.Once);
		_membershipRepoMock.Verify(m => m.GetMembershipUserRegistered(It.IsAny<Guid>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Get<bool?>(It.IsAny<string>()), Times.Once);
		_membershipRepoMock.Verify(repo => repo.GetMembershipById(It.IsAny<Guid>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<TimeSpan>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Get<PostJob>(It.IsAny<string>()), Times.Once);
		_postRepoMock.Verify(repo => repo.GetLatestPosts(), Times.Never);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<PostJob>(), It.IsAny<TimeSpan>()), Times.Never);
		_eventBusRabbitMQProducerMock.Verify(e =>
			e.Publish(EventBus.PostNotificationQueue, It.IsAny<PostCreatedEvent>()), Times.Once);
	}

	[Fact]
	public async Task CheckNotificationWhenPostAsync_Should_ReturnSuccess_WhenGettingMembershipFromRepoIfNotCached()
	{
		// Arrange
		var membershipUser = new MembershipUser { MembershipId = _membershipId, UserId = _userId};
		var post = createFakePostJob();

		_cacheServiceMock.Setup(c => c.Get<string>(It.IsAny<string>()))
						 .ReturnsAsync((string)null);
		_membershipRepoMock.Setup(m => m.GetMembershipUserRegistered(It.IsAny<Guid>()))
						   .ReturnsAsync(membershipUser);
		_cacheServiceMock.Setup(c => c.Set(It.IsAny<string>(), _membershipId.ToString(), TimeSpan.FromMinutes(5)))
						 .Returns(Task.CompletedTask);
		_cacheServiceMock.Setup(c => c.Get<bool?>(It.IsAny<string>()))
						 .ReturnsAsync(true);
		_cacheServiceMock.Setup(c => c.Get<PostJob>(It.IsAny<string>()))
						 .ReturnsAsync(post);
		_eventBusRabbitMQProducerMock.Setup(rbmq =>
			rbmq.Publish(EventBus.PostNotificationQueue, It.IsAny<PostCreatedEvent>()));

		// Act
		var result = await _userServ.CheckNotificationWhenPost(_userId);
		var jsonResult = Assert.IsType<JsonResult>(result);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		_cacheServiceMock.Verify(c => c.Get<string>(It.IsAny<string>()), Times.Once);
		_membershipRepoMock.Verify(m => m.GetMembershipUserRegistered(It.IsAny<Guid>()), Times.Once);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Once);
		_cacheServiceMock.Verify(c => c.Get<bool?>(It.IsAny<string>()), Times.Once);
		_membershipRepoMock.Verify(repo => repo.GetMembershipById(It.IsAny<Guid>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<TimeSpan>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Get<PostJob>(It.IsAny<string>()), Times.Once);
		_postRepoMock.Verify(repo => repo.GetLatestPosts(), Times.Never);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<PostJob>(), It.IsAny<TimeSpan>()), Times.Never);
		_eventBusRabbitMQProducerMock.Verify(e =>
			e.Publish(EventBus.PostNotificationQueue, It.IsAny<PostCreatedEvent>()), Times.Once);
	}

	[Fact]
	public async Task CheckNotificationWhenPostAsync_Should_ReturnSuccess_WhenGettingMembershipDetailsIfNotCached()
	{
		// Arrange
		var membership = new Membership
			{ Id = _membershipId, Description = "Test", Name = "Test Package", ExpireTime = 3, Price = 1 };
		var post = createFakePostJob();

		_cacheServiceMock.Setup(c => c.Get<string>(It.IsAny<string>()))
						 .ReturnsAsync(_membershipId.ToString());
		_cacheServiceMock.Setup(c => c.Get<bool?>(It.IsAny<string>()))
						 .ReturnsAsync((bool?)null);
		_membershipRepoMock.Setup(m => m.GetMembershipById(It.IsAny<Guid>()))
						   .ReturnsAsync(membership);
		_cacheServiceMock.Setup(c => c.Set(It.IsAny<string>(), true, TimeSpan.FromMinutes(5)))
			.Returns(Task.CompletedTask);
		_cacheServiceMock.Setup(c => c.Get<PostJob>(It.IsAny<string>()))
			.ReturnsAsync(post);
		_eventBusRabbitMQProducerMock.Setup(rbmq =>
			rbmq.Publish(EventBus.PostNotificationQueue, It.IsAny<PostCreatedEvent>()));

		// Act
		var result = await _userServ.CheckNotificationWhenPost(_userId);
		var jsonResult = Assert.IsType<JsonResult>(result);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		_cacheServiceMock.Verify(c => c.Get<string>(It.IsAny<string>()), Times.Once);
		_membershipRepoMock.Verify(m => m.GetMembershipUserRegistered(It.IsAny<Guid>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Get<bool?>(It.IsAny<string>()), Times.Once);
		_membershipRepoMock.Verify(repo => repo.GetMembershipById(It.IsAny<Guid>()), Times.Once);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<TimeSpan>()), Times.Once);
		_cacheServiceMock.Verify(c => c.Get<PostJob>(It.IsAny<string>()), Times.Once);
		_postRepoMock.Verify(repo => repo.GetLatestPosts(), Times.Never);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<PostJob>(), It.IsAny<TimeSpan>()), Times.Never);
		_eventBusRabbitMQProducerMock.Verify(e =>
			e.Publish(EventBus.PostNotificationQueue, It.IsAny<PostCreatedEvent>()), Times.Once);
	}

	[Fact]
	public async Task CheckNotificationWhenPostAsync_Should_ReturnSuccess_WhenGettingLatestPostIfNotCached()
	{
		// Arrange
		var post = createFakePostJob();

		_cacheServiceMock.Setup(c => c.Get<string>(It.IsAny<string>()))
			.ReturnsAsync(_membershipId.ToString());
		_cacheServiceMock.Setup(c => c.Get<bool?>(It.IsAny<string>()))
			.ReturnsAsync(true);
		_cacheServiceMock.Setup(c => c.Get<PostJob>(It.IsAny<string>()))
			.ReturnsAsync((PostJob)null);
		_postRepoMock.Setup(p => p.GetLatestPosts())
			.ReturnsAsync(post);
		_cacheServiceMock.Setup(cache => cache.Set(It.IsAny<string>(), post, TimeSpan.FromMinutes(5)))
			.Returns(Task.CompletedTask);
		_eventBusRabbitMQProducerMock.Setup(rbmq =>
			rbmq.Publish(EventBus.PostNotificationQueue, It.IsAny<PostCreatedEvent>()));

		// Act
		var result = await _userServ.CheckNotificationWhenPost(_userId);
		var jsonResult = Assert.IsType<JsonResult>(result);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		_cacheServiceMock.Verify(c => c.Get<string>(It.IsAny<string>()), Times.Once);
		_membershipRepoMock.Verify(m => m.GetMembershipUserRegistered(It.IsAny<Guid>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Get<bool?>(It.IsAny<string>()), Times.Once);
		_membershipRepoMock.Verify(repo => repo.GetMembershipById(It.IsAny<Guid>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<TimeSpan>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Get<PostJob>(It.IsAny<string>()), Times.Once);
		_postRepoMock.Verify(repo => repo.GetLatestPosts(), Times.Once);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<PostJob>(), It.IsAny<TimeSpan>()), Times.Once);
		_eventBusRabbitMQProducerMock.Verify(e =>
			e.Publish(EventBus.PostNotificationQueue, It.IsAny<PostCreatedEvent>()), Times.Once);
	}

	[Fact]
	public async Task CheckNotificationWhenPostAsync_Should_ThrowException_IfUserNotRegistered()
	{
		// Arrange
		_cacheServiceMock.Setup(c => c.Get<string>(It.IsAny<string>()))
						 .ReturnsAsync((string)null);
		_membershipRepoMock.Setup(m => m.GetMembershipUserRegistered(It.IsAny<Guid>()))
						   .ReturnsAsync((MembershipUser)null);

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.CheckNotificationWhenPost(_userId));
		_cacheServiceMock.Verify(c => c.Get<string>(It.IsAny<string>()), Times.Once);
		_membershipRepoMock.Verify(m => m.GetMembershipUserRegistered(It.IsAny<Guid>()), Times.Once);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Get<bool?>(It.IsAny<string>()), Times.Never);
		_membershipRepoMock.Verify(repo => repo.GetMembershipById(It.IsAny<Guid>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<TimeSpan>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Get<PostJob>(It.IsAny<string>()), Times.Never);
		_postRepoMock.Verify(repo => repo.GetLatestPosts(), Times.Never);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<PostJob>(), It.IsAny<TimeSpan>()), Times.Never);
		_eventBusRabbitMQProducerMock.Verify(e =>
			e.Publish(EventBus.PostNotificationQueue, It.IsAny<PostCreatedEvent>()), Times.Never);
	}

	[Fact]
	public async Task CheckNotificationWhenPostAsync_Should_ThrowException_IfMembershipNotFound()
	{
		// Arrange
		_cacheServiceMock.Setup(c => c.Get<string>(It.IsAny<string>()))
						 .ReturnsAsync(_membershipId.ToString());
		_cacheServiceMock.Setup(c => c.Get<bool?>(It.IsAny<string>()))
						 .ReturnsAsync((bool?)null);
		_membershipRepoMock.Setup(m => m.GetMembershipById(It.IsAny<Guid>()))
						   .ReturnsAsync((Membership)null);

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.CheckNotificationWhenPost(_userId));
		_cacheServiceMock.Verify(c => c.Get<string>(It.IsAny<string>()), Times.Once);
		_membershipRepoMock.Verify(m => m.GetMembershipUserRegistered(It.IsAny<Guid>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Get<bool?>(It.IsAny<string>()), Times.Once);
		_membershipRepoMock.Verify(repo => repo.GetMembershipById(It.IsAny<Guid>()), Times.Once);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<TimeSpan>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Get<PostJob>(It.IsAny<string>()), Times.Never);
		_postRepoMock.Verify(repo => repo.GetLatestPosts(), Times.Never);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<PostJob>(), It.IsAny<TimeSpan>()), Times.Never);
		_eventBusRabbitMQProducerMock.Verify(e =>
			e.Publish(EventBus.PostNotificationQueue, It.IsAny<PostCreatedEvent>()), Times.Never);
	}

	[Fact]
	public async Task CheckNotificationWhenPostAsync_Should_ThrowException_IfNoRecentPostAvailable()
	{
		// Arrange
		_cacheServiceMock.Setup(c => c.Get<string>(It.IsAny<string>()))
			.ReturnsAsync(_membershipId.ToString());
		_cacheServiceMock.Setup(c => c.Get<bool?>(It.IsAny<string>()))
			.ReturnsAsync(true);
		_cacheServiceMock.Setup(c => c.Get<PostJob>(It.IsAny<string>()))
						 .ReturnsAsync((PostJob)null);
		_postRepoMock.Setup(p => p.GetLatestPosts())
					 .ReturnsAsync((PostJob)null);

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.CheckNotificationWhenPost(_userId));
		_cacheServiceMock.Verify(c => c.Get<string>(It.IsAny<string>()), Times.Once);
		_membershipRepoMock.Verify(m => m.GetMembershipUserRegistered(It.IsAny<Guid>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Get<bool?>(It.IsAny<string>()), Times.Once);
		_membershipRepoMock.Verify(repo => repo.GetMembershipById(It.IsAny<Guid>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<TimeSpan>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Get<PostJob>(It.IsAny<string>()), Times.Once);
		_postRepoMock.Verify(repo => repo.GetLatestPosts(), Times.Once);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<PostJob>(), It.IsAny<TimeSpan>()), Times.Never);
		_eventBusRabbitMQProducerMock.Verify(e =>
			e.Publish(EventBus.PostNotificationQueue, It.IsAny<PostCreatedEvent>()), Times.Never);
	}
}
