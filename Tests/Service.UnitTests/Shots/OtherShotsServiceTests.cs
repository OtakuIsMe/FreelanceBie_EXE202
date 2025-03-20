using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Shots;
public class OtherShotsServiceTests
{
	private readonly Mock<IShotRepo> _shotRepoMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly Mock<ISpecialtyRepo> _specialtyRepoMock;
	private readonly ShotServ _shotServ;
	private readonly Guid _userId = Guid.NewGuid();
	private readonly Guid _shotId = Guid.NewGuid();

	public OtherShotsServiceTests()
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
	public async Task OtherShotsAsync_Should_ReturnSuccess_WhenGettingOtherShotsSuccess()
	{
		// Arrange
		var shots = new List<Shot>
		{
			new Shot
			{
				Id = _shotId,
				UserId = _userId,
				Html = "<h1>Nothing</h1>",
				Title = "Test",
				View = 0
			}
		};

		_shotRepoMock.Setup(repo => repo.GetShotById(It.IsAny<Guid>()))
			.ReturnsAsync(shots[0]);
		_shotRepoMock.Setup(repo => repo.GetShotsByUser(It.IsAny<Guid>()))
			.ReturnsAsync(shots);

		// Act
		var result = await _shotServ.OtherShots(_shotId);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.NotNull(jsonResult);
		Assert.Equal(200, jsonResult.StatusCode);
		_shotRepoMock.Verify(repo => repo.GetShotById(It.IsAny<Guid>()), Times.Once);
		_shotRepoMock.Verify(repo => repo.GetShotsByUser(It.IsAny<Guid>()), Times.Once);
	}

	[Fact]
	public async Task OtherShotsAsync_Should_ReturnFail_WhenGettingOtherShotsFails()
	{
		// Arrange
		_shotRepoMock.Setup(repo => repo.GetShotById(It.IsAny<Guid>()))
			.ReturnsAsync((Shot)null);

		// Act
		var result = await _shotServ.OtherShots(_shotId);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.NotNull(jsonResult);
		Assert.Equal(400, jsonResult.StatusCode);
		_shotRepoMock.Verify(repo => repo.GetShotById(It.IsAny<Guid>()), Times.Once);
		_shotRepoMock.Verify(repo => repo.GetShotsByUser(It.IsAny<Guid>()), Times.Never);
	}

	[Fact]
	public async Task OtherShotsAsync_Should_ReturnEmptyList_When_UserHasNoOtherShots()
	{
		// Arrange
		var shot = new Shot
		{
			Id = _shotId,
			UserId = Guid.Empty,
			Html = "<h1>Nothing</h1>",
			Title = "Test",
			View = 0
		};

		var shots = new List<Shot> { };

		_shotRepoMock.Setup(repo => repo.GetShotById(It.IsAny<Guid>()))
			.ReturnsAsync(shot);
		_shotRepoMock.Setup(repo => repo.GetShotsByUser(It.IsAny<Guid>()))
			.ReturnsAsync(shots);

		// Act
		var result = await _shotServ.OtherShots(_shotId);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.NotNull(jsonResult);
		var returnedShots = Assert.IsType<List<Shot>>(jsonResult.Value);
		Assert.Empty(returnedShots);
		_shotRepoMock.Verify(repo => repo.GetShotById(It.IsAny<Guid>()), Times.Once);
		_shotRepoMock.Verify(repo => repo.GetShotsByUser(It.IsAny<Guid>()), Times.Once);
	}

	[Fact]
	public async Task OtherShotsAsync_Should_ReturnMaxFourShots_When_UserHasMoreThanFour()
	{
		// Arrange
		var shot = new Shot
		{
			Id = _shotId,
			UserId = _userId,
			Html = "<h1>Nothing</h1>",
			Title = "Test",
			View = 0
		};

		var shots = Enumerable.Range(1, 10).Select(i => new Shot
		{
			Id = Guid.NewGuid(),
			UserId = _userId,
			Html = "<h1>Nothing</h1>",
			Title = "Test",
			View = 0
		}).ToList();
		shots.Insert(0, shot);

		_shotRepoMock.Setup(repo => repo.GetShotById(It.IsAny<Guid>()))
			.ReturnsAsync(shot);
		_shotRepoMock.Setup(repo => repo.GetShotsByUser(It.IsAny<Guid>()))
			.ReturnsAsync(shots);

		// Act
		var result = await _shotServ.OtherShots(_shotId);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.NotNull(jsonResult);
		var returnedShots = Assert.IsType<List<Shot>>(jsonResult.Value);
		Assert.Equal(4, returnedShots.Count);
	}

	[Fact]
	public async Task OtherShotsAsync_Should_ReturnBadRequest_WhenGettingOtherShotsExceptionOccurs()
	{
		// Arrange
		_shotRepoMock.Setup(repo => repo.GetShotById(It.IsAny<Guid>()))
			.ThrowsAsync(new Exception());

		// Act
		var result = await _shotServ.OtherShots(_shotId);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.NotNull(jsonResult);
		Assert.Equal(400, jsonResult.StatusCode);
	}
}
