using BE.src.api.domains.Enum;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Communications;
public class GetInTouchServiceTests
{
	private readonly Mock<ICommunicationRepo> _communicationRepoMock;
	private readonly Mock<IPostRepo> _postRepoMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly CommunicationServ _communicationServ;
	private readonly Guid _userId = Guid.NewGuid();
	private readonly Guid _postId = Guid.NewGuid();
	private readonly string _message = "Hello!";
	public GetInTouchServiceTests()
	{
		_communicationRepoMock = new Mock<ICommunicationRepo>();
		_postRepoMock = new Mock<IPostRepo>();
		_cacheServiceMock = new Mock<ICacheService>();
		_communicationServ = new CommunicationServ(
			_communicationRepoMock.Object,
			_postRepoMock.Object,
			_cacheServiceMock.Object);
	}

	[Fact]
	public async Task GetInTouchAsync_Should_ReturnSuccess_WhenCommunicationIsSuccessful()
	{
		// Arrange
		var post = new PostJob
		{
			UserId = Guid.NewGuid(),
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

		_postRepoMock.Setup(p => p.GetPostById(It.IsAny<Guid>(), default)).ReturnsAsync(post);
		_communicationRepoMock.Setup(c => c.AddCommunication(It.IsAny<Communication>())).ReturnsAsync(true);
		_communicationRepoMock.Setup(c => c.AddMessage(It.IsAny<Message>())).ReturnsAsync(true);

		// Act
		var result = await _communicationServ.GetInTouch(_userId, _postId, _message);
		var jsonResult = Assert.IsType<JsonResult>(result);

		// Assert
		Assert.Equal(201, jsonResult.StatusCode);
		_postRepoMock.Verify(p => p.GetPostById(It.IsAny<Guid>(), default), Times.Once);
		_communicationRepoMock.Verify(c => c.AddCommunication(It.IsAny<Communication>()), Times.Once);
		_communicationRepoMock.Verify(c => c.AddMessage(It.IsAny<Message>()), Times.Once);
	}

	[Fact]
	public async Task GetInTouchAsync_Should_ThrowException_WhenPostNotFound()
	{
		// Arrange
		_postRepoMock.Setup(p => p.GetPostById(It.IsAny<Guid>(), default)).ReturnsAsync((PostJob)null);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _communicationServ.GetInTouch(_userId, _postId, _message));
		_postRepoMock.Verify(p => p.GetPostById(It.IsAny<Guid>(), default), Times.Once);
		_communicationRepoMock.Verify(c => c.AddCommunication(It.IsAny<Communication>()), Times.Never);
		_communicationRepoMock.Verify(c => c.AddMessage(It.IsAny<Message>()), Times.Never);
	}

	[Fact]
	public async Task GetInTouchAsync_Should_ThrowException_WhenCommunicationFails()
	{
		// Arrange
		var post = new PostJob
		{
			UserId = Guid.NewGuid(),
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
		_postRepoMock.Setup(p => p.GetPostById(It.IsAny<Guid>(), default)).ReturnsAsync(post);
		_communicationRepoMock.Setup(c => c.AddCommunication(It.IsAny<Communication>())).ReturnsAsync(false);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _communicationServ.GetInTouch(_userId, _postId, _message));
		_postRepoMock.Verify(p => p.GetPostById(It.IsAny<Guid>(), default), Times.Once);
		_communicationRepoMock.Verify(c => c.AddCommunication(It.IsAny<Communication>()), Times.Once);
		_communicationRepoMock.Verify(c => c.AddMessage(It.IsAny<Message>()), Times.Never);
	}

	[Fact]
	public async Task GetInTouchAsync_Should_ThrowException_WhenMessageFails()
	{
		// Arrange
		var post = new PostJob
		{
			UserId = Guid.NewGuid(),
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
		_postRepoMock.Setup(p => p.GetPostById(It.IsAny<Guid>(), default)).ReturnsAsync(post);
		_communicationRepoMock.Setup(c => c.AddCommunication(It.IsAny<Communication>())).ReturnsAsync(true);
		_communicationRepoMock.Setup(c => c.AddMessage(It.IsAny<Message>())).ReturnsAsync(false);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _communicationServ.GetInTouch(_userId, _postId, _message));
		_postRepoMock.Verify(p => p.GetPostById(It.IsAny<Guid>(), default), Times.Once);
		_communicationRepoMock.Verify(c => c.AddCommunication(It.IsAny<Communication>()), Times.Once);
		_communicationRepoMock.Verify(c => c.AddMessage(It.IsAny<Message>()), Times.Once);
	}

	[Fact]
	public async Task GetInTouchAsync_Should_ThrowException_WhenRepositoryThrowsException()
	{
		// Arrange
		_postRepoMock.Setup(p => p.GetPostById(It.IsAny<Guid>(), default)).ThrowsAsync(new Exception());

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _communicationServ.GetInTouch(_userId, _postId, _message));
		_postRepoMock.Verify(p => p.GetPostById(It.IsAny<Guid>(), default), Times.Once);
		_communicationRepoMock.Verify(c => c.AddCommunication(It.IsAny<Communication>()), Times.Never);
		_communicationRepoMock.Verify(c => c.AddMessage(It.IsAny<Message>()), Times.Never);
	}
}
