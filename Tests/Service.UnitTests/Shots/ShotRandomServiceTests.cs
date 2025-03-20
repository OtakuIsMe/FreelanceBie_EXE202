using BE.src.api.domains.DTOs.Shot;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Shots;
public class ShotRandomServiceTests
{
	private readonly Mock<IShotRepo> _shotRepoMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly Mock<ISpecialtyRepo> _specialtyRepoMock;
	private readonly ShotServ _shotServ;
	private readonly Guid _shotId = Guid.NewGuid();

	public ShotRandomServiceTests()
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
	public async Task ShotRandomAsync_Should_ReturnSuccess_WhenGettingShotRandomSuccess()
	{
		// Arrange

		var shots = new List<ShotView>
		{
			new ShotView
			{
				Id = _shotId,
				User = new UserShotCard
				{
					Username = "user",
					Image = "https://image_user.com"
				},
				Title = "Test",
				CountLike = 1,
				CountView = 1,
				Image = "https://image_shot.com"
			}
		};

		_shotRepoMock.Setup(repo => repo.GetShotRandom(It.IsAny<int>()))
			.ReturnsAsync(shots);

		// Act
		var result = await _shotServ.ShotRandom(1);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.NotNull(jsonResult);
		Assert.Equal(200, jsonResult.StatusCode);
		_shotRepoMock.Verify(repo => repo.GetShotRandom(It.IsAny<int>()), Times.Once);
	}

	[Fact]
	public async Task ShotRandomAsync_Should_ReturnSuccess_WhenReturnRequestedNumberOfShots()
	{
		// Arrange
		var expectedListCount = 3;
		var shots = Enumerable.Range(1, expectedListCount).Select(i => new ShotView
		{
			Id = Guid.NewGuid(),
			User = new UserShotCard
			{
				Username = $"user{i}",
				Image = $"https://image_user{i}.com"
			},
			Title = $"Shot {i}",
			CountLike = i,
			CountView = i * 2,
			Image = $"https://image_shot{i}.com"
		}).ToList();

		_shotRepoMock.Setup(repo => repo.GetShotRandom(It.IsAny<int>()))
			.ReturnsAsync(shots);

		// Act
		var result = await _shotServ.ShotRandom(expectedListCount);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.NotNull(jsonResult);
		Assert.Equal(200, jsonResult.StatusCode);
		var returnedResult = Assert.IsType<List<ShotView>>(jsonResult.Value);
		Assert.Equal(expectedListCount, returnedResult.Count);
		_shotRepoMock.Verify(repo => repo.GetShotRandom(It.IsAny<int>()), Times.Once);
	}

	[Fact]
	public async Task ShotRandomAsync_Should_ReturnEmptyList_WhenGettingShotRandomWithEmptyList()
	{
		// Arrange
		_shotRepoMock.Setup(repo => repo.GetShotRandom(It.IsAny<int>()))
			.ReturnsAsync(new List<ShotView>());

		// Act
		var result = await _shotServ.ShotRandom(1);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.NotNull(jsonResult);
		var returnedResult = Assert.IsType<List<ShotView>>(jsonResult.Value);
		Assert.Empty(returnedResult);
		_shotRepoMock.Verify(repo => repo.GetShotRandom(It.IsAny<int>()), Times.Once);
	}

	[Fact]
	public async Task ShotRandomAsync_Should_ReturnFails_WhenGettingShotRandomWithExceptionOccurs()
	{
		// Arrange
		_shotRepoMock.Setup(repo => repo.GetShotRandom(It.IsAny<int>()))
			.ThrowsAsync(new Exception());

		// Act
		var result = await _shotServ.ShotRandom(1);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.NotNull(jsonResult);
		Assert.Equal(400, jsonResult.StatusCode);
		_shotRepoMock.Verify(repo => repo.GetShotRandom(It.IsAny<int>()), Times.Once);
	}
}
