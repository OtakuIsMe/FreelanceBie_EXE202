using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Shots;
public class LikeShotServiceTests
{
	private readonly Mock<IShotRepo> _shotRepoMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly Mock<ISpecialtyRepo> _specialtyRepoMock;
	private readonly ShotServ _shotServ;
	private readonly Guid _userId = Guid.NewGuid();
	private readonly Guid _shotId = Guid.NewGuid();
	public LikeShotServiceTests()
	{
		_shotRepoMock = new Mock<IShotRepo>();
		_cacheServiceMock = new Mock<ICacheService>();
		_specialtyRepoMock = new Mock<ISpecialtyRepo>();
		_shotServ = new ShotServ(
			_shotRepoMock.Object,
			_cacheServiceMock.Object,
			_specialtyRepoMock.Object);
	}

	[Fact]
	public async Task LikeShotAsync_Should_ReturnSuccess_WhenStateIsTrue()
	{
		// Arrange
		_shotRepoMock.Setup(repo => repo.GetLike(_userId, _shotId))
			.ReturnsAsync((Like)null);
		_shotRepoMock.Setup(repo => repo.CreateLikeShot(It.IsAny<Like>()))
			.ReturnsAsync(true);

		// Act
		var result = await _shotServ.LikeShot(_userId, _shotId, true);

		// Assert
		Assert.NotNull(result);
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.Equal(200, jsonResult.StatusCode);
		_shotRepoMock.Verify(repo => repo.GetLike(_userId, _shotId), Times.Once);
		_shotRepoMock.Verify(repo => repo.CreateLikeShot(It.IsAny<Like>()), Times.Once);
		_shotRepoMock.Verify(repo => repo.DeleteLikeShot(It.IsAny<Like>()), Times.Never);
	}

	[Fact]
	public async Task LikeShotAsync_Should_ReturnSuccess_WhenStateIsFalse()
	{
		// Arrange
		var like = new Like
		{
			UserId = _userId,
			ShotId = _shotId
		};

		_shotRepoMock.Setup(repo => repo.GetLike(_userId, _shotId))
			.ReturnsAsync(like);
		_shotRepoMock.Setup(repo => repo.DeleteLikeShot(It.IsAny<Like>()))
			.ReturnsAsync(true);

		// Act
		var result = await _shotServ.LikeShot(_userId, _shotId, false);

		// Assert
		Assert.NotNull(result);
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.Equal(200, jsonResult.StatusCode);
		_shotRepoMock.Verify(repo => repo.GetLike(_userId, _shotId), Times.Once);
		_shotRepoMock.Verify(repo => repo.CreateLikeShot(It.IsAny<Like>()), Times.Never);
		_shotRepoMock.Verify(repo => repo.DeleteLikeShot(It.IsAny<Like>()), Times.Once);
	}

	[Fact]
	public async Task LikeShotAsync_Should_ThrowException_WhenGettingDatabaseFails()
	{
		// Arrange
		var like = new Like
		{
			UserId = _userId,
			ShotId = _shotId
		};

		_shotRepoMock.Setup(repo => repo.GetLike(_userId, _shotId))
			.ReturnsAsync(like);
		_shotRepoMock.Setup(repo => repo.DeleteLikeShot(It.IsAny<Like>()))
			.Throws(new Exception());

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _shotServ.LikeShot(_userId, _shotId, false));
		_shotRepoMock.Verify(repo => repo.GetLike(_userId, _shotId), Times.Once);
		_shotRepoMock.Verify(repo => repo.CreateLikeShot(It.IsAny<Like>()), Times.Never);
		_shotRepoMock.Verify(repo => repo.DeleteLikeShot(It.IsAny<Like>()), Times.Once);
	}
}
