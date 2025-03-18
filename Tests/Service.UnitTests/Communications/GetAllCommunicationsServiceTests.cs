using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Communications;
public class GetAllCommunicationsServiceTests
{
	private readonly Mock<ICommunicationRepo> _communicationRepoMock;
	private readonly Mock<IPostRepo> _postRepoMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly CommunicationServ _communicationServ;
	private readonly Guid _userId = Guid.NewGuid();
	public GetAllCommunicationsServiceTests()
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
	public async Task GetAllCommunicationsAsync_Should_ReturnSuccess_WhenCacheIsAvailable()
	{
		// Arrange
		var expectedCommunications = new List<Communication>
		{
			new Communication { Id = Guid.NewGuid(),  FirstId = _userId, ZeroId = Guid.NewGuid()}
		};

		_cacheServiceMock.Setup(c => c.Get<List<Communication>>(It.IsAny<string>()))
			.ReturnsAsync(expectedCommunications);

		// Act
		var result = await _communicationServ.GetAllCommunications(_userId);
		var jsonResult = Assert.IsType<JsonResult>(result);
		var returnedCommunications = Assert.IsType<List<Communication>>(jsonResult.Value);

		// Assert
		Assert.Equal(expectedCommunications.Count, returnedCommunications.Count);
		_cacheServiceMock.Verify(c => c.Get<List<Communication>>(It.IsAny<string>()), Times.Once);
		_communicationRepoMock.Verify(c => c.GetCommunications(It.IsAny<Guid>()), Times.Never);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), expectedCommunications, TimeSpan.FromMinutes(10)), Times.Never);
	}

	[Fact]
	public async Task GetAllCommunicationsAsync_Should_ReturnSuccess_WhenCacheIsEmpty()
	{
		// Arrange
		var expectedCommunications = new List<Communication>
		{
			new Communication { Id = Guid.NewGuid(),  FirstId = _userId, ZeroId = Guid.NewGuid()}
		};

		_cacheServiceMock.Setup(c => c.Get<List<Communication>>(It.IsAny<string>()))
			.ReturnsAsync((List<Communication>)null);
		_communicationRepoMock.Setup(r => r.GetCommunications(_userId))
			.ReturnsAsync(expectedCommunications);
		_cacheServiceMock.Setup(c => c.Set(It.IsAny<string>(), expectedCommunications, TimeSpan.FromMinutes(10)))
			.Returns(Task.CompletedTask);

		// Act
		var result = await _communicationServ.GetAllCommunications(_userId);
		var jsonResult = Assert.IsType<JsonResult>(result);
		var returnedCommunications = Assert.IsType<List<Communication>>(jsonResult.Value);

		// Assert
		Assert.Single(returnedCommunications);
		Assert.Equal(_userId, returnedCommunications[0].FirstId);
		_cacheServiceMock.Verify(c => c.Get<List<Communication>>(It.IsAny<string>()), Times.Once);
		_communicationRepoMock.Verify(c => c.GetCommunications(It.IsAny<Guid>()), Times.Once);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), expectedCommunications, TimeSpan.FromMinutes(10)), Times.Once);
	}

	[Fact]
	public async Task GetAllCommunicationsAsync_Should_ThrowException_WhenNoCommunicationsFound()
	{
		// Arrange
		var expectedCommunications = new List<Communication> { };

		_cacheServiceMock.Setup(c => c.Get<List<Communication>>(It.IsAny<string>()))
			.ReturnsAsync((List<Communication>)null);
		_communicationRepoMock.Setup(r => r.GetCommunications(_userId))
			.ReturnsAsync((List<Communication>)null);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _communicationServ.GetAllCommunications(_userId));
		_cacheServiceMock.Verify(c => c.Get<List<Communication>>(It.IsAny<string>()), Times.Once);
		_communicationRepoMock.Verify(c => c.GetCommunications(It.IsAny<Guid>()), Times.Once);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), expectedCommunications, TimeSpan.FromMinutes(10)), Times.Never);
	}

	[Fact]
	public async Task GetAllCommunicationsAsync_Should_ThrowException_WhenRepositoryThrowsException()
	{
		// Arrange
		var expectedCommunications = new List<Communication> { };

		_cacheServiceMock.Setup(c => c.Get<List<Communication>>(It.IsAny<string>()))
			.ReturnsAsync((List<Communication>)null);
		_communicationRepoMock.Setup(r => r.GetCommunications(It.IsAny<Guid>()))
			.ThrowsAsync(new Exception());

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _communicationServ.GetAllCommunications(_userId));
		_cacheServiceMock.Verify(c => c.Get<List<Communication>>(It.IsAny<string>()), Times.Once);
		_communicationRepoMock.Verify(c => c.GetCommunications(It.IsAny<Guid>()), Times.Once);
		_cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), expectedCommunications, TimeSpan.FromMinutes(10)), Times.Never);
	}
}
