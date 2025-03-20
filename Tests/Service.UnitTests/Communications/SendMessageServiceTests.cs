using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Communications;
public class SendMessageServiceTests
{
	private readonly Mock<ICommunicationRepo> _communicationRepoMock;
	private readonly Mock<IPostRepo> _postRepoMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly CommunicationServ _communicationServ;
	private readonly Guid _communicationId = Guid.NewGuid();
	private readonly Guid _userId = Guid.NewGuid();
	private readonly string _message = "Hello!";
	public SendMessageServiceTests()
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
	public async Task SendMessageAsync_Should_ReturnSuccess_WhenMessageIsSentSuccessfully()
	{
		// Arrange
		var communication = new Communication { Id = _communicationId, ZeroId = _userId, FirstId = Guid.NewGuid()};
		var messages = new List<Message>
		{
			new Message { Id = Guid.NewGuid(), Content = "Test", CommunicationId = _communicationId, Index = 0, PersonIndex = true}
		};

		_communicationRepoMock.Setup(r => r.GetCommunicationById(It.IsAny<Guid>(), default))
			.ReturnsAsync(communication);
		_communicationRepoMock.Setup(r => r.GetMessages(It.IsAny<Guid>()))
			.ReturnsAsync(messages);
		_communicationRepoMock.Setup(r => r.AddMessage(It.IsAny<Message>()))
			.ReturnsAsync(true);
		_cacheServiceMock.Setup(c => c.Remove(It.IsAny<string>()))
			.Returns(Task.CompletedTask);

		// Act
		var result = await _communicationServ.SendMessage(_communicationId, _userId, _message);
		var jsonResult = Assert.IsType<JsonResult>(result);

		// Assert
		Assert.Equal(201, jsonResult.StatusCode);
		_communicationRepoMock.Verify(r => r.GetCommunicationById(It.IsAny<Guid>(), default), Times.Once);
		_communicationRepoMock.Verify(r => r.GetMessages(It.IsAny<Guid>()), Times.Once);
		_communicationRepoMock.Verify(r => r.AddMessage(It.IsAny<Message>()), Times.Once);
		_cacheServiceMock.Verify(c => c.Remove(It.IsAny<string>()), Times.Once);
	}

	[Fact]
	public async Task SendMessageAsync_Should_ThrowException_WhenCommunicationNotFound()
	{
		// Arrange
		_communicationRepoMock.Setup(r => r.GetCommunicationById(It.IsAny<Guid>(), default))
			.ReturnsAsync((Communication)null);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _communicationServ.SendMessage(_communicationId, _userId, _message));
		_communicationRepoMock.Verify(r => r.GetCommunicationById(It.IsAny<Guid>(), default), Times.Once);
		_communicationRepoMock.Verify(r => r.GetMessages(It.IsAny<Guid>()), Times.Never);
		_communicationRepoMock.Verify(r => r.AddMessage(It.IsAny<Message>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Remove(It.IsAny<string>()), Times.Never);
	}

	[Fact]
	public async Task SendMessageAsync_Should_ThrowException_WhenMessagesNotFound()
	{
		// Arrange
		var communication = new Communication { Id = _communicationId, ZeroId = _userId, FirstId = Guid.NewGuid() };
		_communicationRepoMock.Setup(r => r.GetCommunicationById(It.IsAny<Guid>(), default))
			.ReturnsAsync(communication);
		_communicationRepoMock.Setup(r => r.GetMessages(It.IsAny<Guid>()))
			.ReturnsAsync((List<Message>)null);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _communicationServ.SendMessage(_communicationId, _userId, _message));
		_communicationRepoMock.Verify(r => r.GetCommunicationById(It.IsAny<Guid>(), default), Times.Once);
		_communicationRepoMock.Verify(r => r.GetMessages(It.IsAny<Guid>()), Times.Once);
		_communicationRepoMock.Verify(r => r.AddMessage(It.IsAny<Message>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Remove(It.IsAny<string>()), Times.Never);
	}

	[Fact]
	public async Task SendMessageAsync_Should_ThrowException_WhenAddMessageFails()
	{
		// Arrange
		var communication = new Communication { Id = _communicationId, ZeroId = _userId, FirstId = Guid.NewGuid()};
		var messages = new List<Message>
		{
			new Message { Id = Guid.NewGuid(), Content = "Test", Index = 0, PersonIndex = true, CommunicationId = _communicationId}
		};

		_communicationRepoMock.Setup(r => r.GetCommunicationById(It.IsAny<Guid>(), default))
			.ReturnsAsync(communication);
		_communicationRepoMock.Setup(r => r.GetMessages(It.IsAny<Guid>()))
			.ReturnsAsync(messages);
		_communicationRepoMock.Setup(r => r.AddMessage(It.IsAny<Message>()))
			.ReturnsAsync(false);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _communicationServ.SendMessage(_communicationId, _userId, _message));
		_communicationRepoMock.Verify(r => r.GetCommunicationById(It.IsAny<Guid>(), default), Times.Once);
		_communicationRepoMock.Verify(r => r.GetMessages(It.IsAny<Guid>()), Times.Once);
		_communicationRepoMock.Verify(r => r.AddMessage(It.IsAny<Message>()), Times.Once);
		_cacheServiceMock.Verify(c => c.Remove(It.IsAny<string>()), Times.Never);
	}

	[Fact]
	public async Task SendMessageAsync_Should_ThrowException_WhenRepositoryThrowsException()
	{
		// Arrange
		_communicationRepoMock.Setup(r => r.GetCommunicationById(It.IsAny<Guid>(), default))
			.ThrowsAsync(new Exception());

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _communicationServ.SendMessage(_communicationId, _userId, _message));
		_communicationRepoMock.Verify(r => r.GetCommunicationById(It.IsAny<Guid>(), default), Times.Once);
		_communicationRepoMock.Verify(r => r.GetMessages(It.IsAny<Guid>()), Times.Never);
		_communicationRepoMock.Verify(r => r.AddMessage(It.IsAny<Message>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Remove(It.IsAny<string>()), Times.Never);
	}
}
