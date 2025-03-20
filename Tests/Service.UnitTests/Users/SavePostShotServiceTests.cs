using System.Text.Json;
using BE.src.api.domains.eventbus.Producers;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Service.UnitTests.Users;
public class SavePostShotServiceTests
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
	private readonly Guid _shotId = Guid.NewGuid();
	private readonly UserServ _userServ;
	public SavePostShotServiceTests()
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
	public async Task SavePostShotAsync_Should_ReturnSuccess_WhenNoExistingSave()
	{
		// Arrange
		_userRepoMock.Setup(repo => repo.GetUserSave(It.IsAny<Guid>(), default))
					 .ReturnsAsync((Save)null);
		_userRepoMock.Setup(repo => repo.CreateSave(It.IsAny<Save>()))
					 .ReturnsAsync(true);

		// Act
		var result = await _userServ.SavePostShot(_userId, _postId, _shotId, true);
		var jsonResult = Assert.IsType<JsonResult>(result);
		var json = JsonSerializer.Serialize(jsonResult.Value);
		var responseObj = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.Equal("Save success", responseObj["Message"].ToString().Trim());
		_userRepoMock.Verify(repo => repo.GetUserSave(It.IsAny<Guid>(), default), Times.Once);
		_userRepoMock.Verify(repo => repo.CreateSave(It.IsAny<Save>()), Times.Once);
		_userRepoMock.Verify(repo => repo.UpdateSave(It.IsAny<Save>()), Times.Never);
		_userRepoMock.Verify(repo => repo.DeleteSave(It.IsAny<Save>()), Times.Never);
	}

	[Fact]
	public async Task SavePostShotAsync_Should_ReturnSuccess_WhenAlreadySaved()
	{
		// Arrange
		var existingSave = new Save { UserId = _userId };

		_userRepoMock.Setup(repo => repo.GetUserSave(It.IsAny<Guid>(), default))
					 .ReturnsAsync(existingSave);
		_userRepoMock.Setup(repo => repo.UpdateSave(It.IsAny<Save>()))
					 .ReturnsAsync(true);

		// Act
		var result = await _userServ.SavePostShot(_userId, _postId, _shotId, true);
		var jsonResult = Assert.IsType<JsonResult>(result);
		var json = JsonSerializer.Serialize(jsonResult.Value);
		var responseObj = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.Equal("Save success", responseObj["Message"].ToString().Trim());
		_userRepoMock.Verify(repo => repo.GetUserSave(It.IsAny<Guid>(), default), Times.Once);
		_userRepoMock.Verify(repo => repo.CreateSave(It.IsAny<Save>()), Times.Never);
		_userRepoMock.Verify(repo => repo.UpdateSave(It.IsAny<Save>()), Times.Once);
		_userRepoMock.Verify(repo => repo.DeleteSave(It.IsAny<Save>()), Times.Never);
	}

	[Fact]
	public async Task SavePostShotAsync_Should_ReturnSuccess_WhenUnSavedIfPostExistsButNotForShot()
	{
		// Arrange
		var existingSave = new Save { UserId = _userId, PostId = _postId, ShotId = _shotId};

		_userRepoMock.Setup(repo => repo.GetUserSave(It.IsAny<Guid>(), default))
					 .ReturnsAsync(existingSave);
		_userRepoMock.Setup(repo => repo.UpdateSave(It.IsAny<Save>()))
			.ReturnsAsync(true);

		// Act
		var result = await _userServ.SavePostShot(_userId, _postId, null, false);
		var jsonResult = Assert.IsType<JsonResult>(result);
		var json = JsonSerializer.Serialize(jsonResult.Value);
		var responseObj = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.Null(existingSave.PostId);
		Assert.Equal("Unsave success", responseObj["Message"].ToString().Trim());
		_userRepoMock.Verify(repo => repo.GetUserSave(It.IsAny<Guid>(), default), Times.Once);
		_userRepoMock.Verify(repo => repo.CreateSave(It.IsAny<Save>()), Times.Never);
		_userRepoMock.Verify(repo => repo.UpdateSave(It.IsAny<Save>()), Times.Once);
		_userRepoMock.Verify(repo => repo.DeleteSave(It.IsAny<Save>()), Times.Never);
	}

	[Fact]
	public async Task SavePostShotAsync_Should_ReturnSuccess_WhenNoPostOrShotLeft()
	{
		// Arrange
		var existingSave = new Save { UserId = _userId};

		_userRepoMock.Setup(repo => repo.GetUserSave(It.IsAny<Guid>(), default))
					 .ReturnsAsync(existingSave);
		_userRepoMock.Setup(repo => repo.DeleteSave(It.IsAny<Save>()))
					 .ReturnsAsync(true);

		// Act
		var result = await _userServ.SavePostShot(_userId, _postId, _shotId, false);
		var jsonResult = Assert.IsType<JsonResult>(result);
		var json = JsonSerializer.Serialize(jsonResult.Value);
		var responseObj = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.Null(existingSave.PostId);
		Assert.Null(existingSave.ShotId);
		Assert.Equal("Unsave success", responseObj["Message"].ToString().Trim());
		_userRepoMock.Verify(repo => repo.GetUserSave(It.IsAny<Guid>(), default), Times.Once);
		_userRepoMock.Verify(repo => repo.CreateSave(It.IsAny<Save>()), Times.Never);
		_userRepoMock.Verify(repo => repo.UpdateSave(It.IsAny<Save>()), Times.Never);
		_userRepoMock.Verify(repo => repo.DeleteSave(It.IsAny<Save>()), Times.Once);
	}

	[Fact]
	public async Task SavePostShotAsync_Should_ReturnSuccess_WhenSaveDoesNotExists()
	{
		// Arrange
		_userRepoMock.Setup(repo => repo.GetUserSave(It.IsAny<Guid>(), default))
					 .ReturnsAsync((Save)null);

		// Act
		var result = await _userServ.SavePostShot(_userId, null, null, false);
		var jsonResult = Assert.IsType<JsonResult>(result);
		var json = JsonSerializer.Serialize(jsonResult.Value);
		var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.Equal("Unsave success", response["Message"].ToString().Trim());
		_userRepoMock.Verify(repo => repo.GetUserSave(It.IsAny<Guid>(), default), Times.Once);
		_userRepoMock.Verify(repo => repo.CreateSave(It.IsAny<Save>()), Times.Never);
		_userRepoMock.Verify(repo => repo.UpdateSave(It.IsAny<Save>()), Times.Never);
		_userRepoMock.Verify(repo => repo.DeleteSave(It.IsAny<Save>()), Times.Never);
	}

	[Fact]
	public async Task SavePostShotAsync_Should_ThrowException_WhenGettingUserSaveFails()
	{
		// Arrange
		_userRepoMock.Setup(repo => repo.GetUserSave(It.IsAny<Guid>(), default))
			.ThrowsAsync(new Exception());

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.SavePostShot(_userId, null, null, false));
		_userRepoMock.Verify(repo => repo.GetUserSave(It.IsAny<Guid>(), default), Times.Once);
		_userRepoMock.Verify(repo => repo.CreateSave(It.IsAny<Save>()), Times.Never);
		_userRepoMock.Verify(repo => repo.UpdateSave(It.IsAny<Save>()), Times.Never);
		_userRepoMock.Verify(repo => repo.DeleteSave(It.IsAny<Save>()), Times.Never);
	}
}
