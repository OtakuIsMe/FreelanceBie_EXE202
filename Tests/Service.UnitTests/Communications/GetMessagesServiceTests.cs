using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Communications;
public class GetMessagesServiceTests
{
	private readonly Mock<ICommunicationRepo> _communicationRepoMock;
	private readonly Mock<IPostRepo> _postRepoMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly CommunicationServ _communicationServ;
	private readonly Guid _communicationId = Guid.NewGuid();
	public GetMessagesServiceTests()
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
	public async Task GetMessagesAsync_Should_ReturnSuccess_WhenCacheIsAvailable()
	{
		// Arrange
		var expectedMessages = new List<Message>
		{
			new Message { Id = Guid.NewGuid(), Content = "Hello", CommunicationId = _communicationId, Index = 0, PersonIndex = true},
		};

		_cacheServiceMock.Setup(c => c.Get<List<Message>>(It.IsAny<string>()))
			.ReturnsAsync(expectedMessages);

		// Act
		var result = await _communicationServ.GetMessages(_communicationId);
		var jsonResult = Assert.IsType<JsonResult>(result);
		var returnedMessages = Assert.IsType<List<Message>>(jsonResult.Value);

		// Assert
		Assert.Equal(expectedMessages.Count, returnedMessages.Count);
		_cacheServiceMock.Verify(c => c.Get<List<Message>>(It.IsAny<string>()), Times.Once);
		_communicationRepoMock.Verify(c => c.GetCommunicationById(It.IsAny<Guid>(), default), Times.Never);
		_communicationRepoMock.Verify(c => c.GetMessages(It.IsAny<Guid>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), expectedMessages, TimeSpan.FromMinutes(10)), Times.Never);
	}

	[Fact]
	public async Task GetMessagesAsync_Should_ReturnSuccess_WhenCacheIsEmpty()
	{
		// Arrange
		var expectedMessages = new List<Message>
		{
			new Message { Id = Guid.NewGuid(), Content = "Test Message", CommunicationId = _communicationId, Index = 0, PersonIndex = true }
		};

		var communication = new Communication
			{ Id = _communicationId, ZeroId = Guid.NewGuid(), FirstId = Guid.NewGuid() };

		_cacheServiceMock.Setup(c => c.Get<List<Message>>(It.IsAny<string>()))
			.ReturnsAsync((List<Message>)null);
		_communicationRepoMock.Setup(r => r.GetCommunicationById(It.IsAny<Guid>(), default))
			.ReturnsAsync(communication);
		_communicationRepoMock.Setup(r => r.GetMessages(It.IsAny<Guid>()))
			.ReturnsAsync(expectedMessages);
		_cacheServiceMock.Setup(c => c.Set(It.IsAny<string>(), expectedMessages, TimeSpan.FromMinutes(10)))
			.Returns(Task.CompletedTask);

		// Act
		var result = await _communicationServ.GetMessages(_communicationId);
		var jsonResult = Assert.IsType<JsonResult>(result);
		var returnedMessages = Assert.IsType<List<Message>>(jsonResult.Value);

		// Assert
		Assert.Single(returnedMessages);
		Assert.Equal("Test Message", returnedMessages[0].Content);
		_cacheServiceMock.Verify(c => c.Get<List<Message>>(It.IsAny<string>()), Times.Once);
		_communicationRepoMock.Verify(c => c.GetCommunicationById(It.IsAny<Guid>(), default), Times.Once);
		_communicationRepoMock.Verify(c => c.GetMessages(It.IsAny<Guid>()), Times.Once);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), expectedMessages, TimeSpan.FromMinutes(10)), Times.Once);
	}

	[Fact]
	public async Task GetMessagesAsync_Should_ThrowException_WhenCommunicationNotFound()
	{
		// Arrange
		var expectedMessages = new List<Message> { };

		_cacheServiceMock.Setup(c => c.Get<List<Message>>(It.IsAny<string>()))
			.ReturnsAsync((List<Message>)null);
		_communicationRepoMock.Setup(r => r.GetCommunicationById(It.IsAny<Guid>(), default))
			.ReturnsAsync((Communication)null);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _communicationServ.GetMessages(_communicationId));
		_cacheServiceMock.Verify(c => c.Get<List<Message>>(It.IsAny<string>()), Times.Once);
		_communicationRepoMock.Verify(c => c.GetCommunicationById(It.IsAny<Guid>(), default), Times.Once);
		_communicationRepoMock.Verify(c => c.GetMessages(It.IsAny<Guid>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), expectedMessages, TimeSpan.FromMinutes(10)), Times.Never);
	}

	[Fact]
	public async Task GetMessagesAsync_Should_ThrowException_WhenMessagesNotFound()
	{
		// Arrange
		var expectedMessages = new List<Message> { };
		var communication = new Communication
			{ Id = _communicationId, ZeroId = Guid.NewGuid(), FirstId = Guid.NewGuid() };

		_cacheServiceMock.Setup(c => c.Get<List<Message>>(It.IsAny<string>()))
			.ReturnsAsync((List<Message>)null);
		_communicationRepoMock.Setup(r => r.GetCommunicationById(It.IsAny<Guid>(), default))
			.ReturnsAsync(communication);
		_communicationRepoMock.Setup(r => r.GetMessages(It.IsAny<Guid>()))
			.ReturnsAsync(new List<Message>());

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _communicationServ.GetMessages(_communicationId));
		_cacheServiceMock.Verify(c => c.Get<List<Message>>(It.IsAny<string>()), Times.Once);
		_communicationRepoMock.Verify(c => c.GetCommunicationById(It.IsAny<Guid>(), default), Times.Once);
		_communicationRepoMock.Verify(c => c.GetMessages(It.IsAny<Guid>()), Times.Once);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), expectedMessages, TimeSpan.FromMinutes(10)), Times.Never);
	}

	[Fact]
	public async Task GetMessagesAsync_Should_ThrowException_WhenRepositoryThrowsException()
	{
		// Arrange
		var expectedMessages = new List<Message> { };

		_cacheServiceMock.Setup(c => c.Get<List<Message>>(It.IsAny<string>()))
			.ReturnsAsync((List<Message>)null);
		_communicationRepoMock.Setup(r => r.GetCommunicationById(It.IsAny<Guid>(), default))
			.ThrowsAsync(new Exception());

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _communicationServ.GetMessages(_communicationId));
		_cacheServiceMock.Verify(c => c.Get<List<Message>>(It.IsAny<string>()), Times.Once);
		_communicationRepoMock.Verify(c => c.GetCommunicationById(It.IsAny<Guid>(), default), Times.Once);
		_communicationRepoMock.Verify(c => c.GetMessages(It.IsAny<Guid>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), expectedMessages, TimeSpan.FromMinutes(10)), Times.Never);
	}
}
