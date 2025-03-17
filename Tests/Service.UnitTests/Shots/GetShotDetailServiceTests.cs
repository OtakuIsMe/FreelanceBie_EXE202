using BE.src.api.domains.Enum;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Shots;
public class GetShotDetailServiceTests
{
	private readonly Mock<IShotRepo> _shotRepoMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly Mock<ISpecialtyRepo> _specialtyRepoMock;
	private readonly ShotServ _shotServ;
	private readonly Guid _userId = Guid.NewGuid();
	private readonly Guid _shotId = Guid.NewGuid();
	public GetShotDetailServiceTests()
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
	public async Task GetShotDetailAsync_Should_ReturnSuccess_WhenGettingShotDetailSuccess()
	{
		// Arrange
		var shot = new Shot
		{
			Id = _shotId,
			UserId = _userId,
			Html = "<h1>Nothing</h1>",
			Title = "Test",
			View = 0,
			ImageVideos = new List<ImageVideo>
				{ new ImageVideo { Url = "http://image.com", IsMain = true, Type = MediaTypeEnum.Image } },
			Specialties = new List<Specialty> { new Specialty { Name = "Design" } },
			User = new User
			{
				Email = "johndoe@example.com",
				Password = "123",
				Role = RoleEnum.Customer,
				Username = "JohnDoe",
				Slogan = "Test",
				ImageVideos = new List<ImageVideo>
					{ new ImageVideo { Url = "http://user.com", Type = MediaTypeEnum.Image, IsMain = true } }
			}
		};

		_shotRepoMock.Setup(repo => repo.GetShotByShotCode(_shotId))
			.ReturnsAsync(shot);
		_shotRepoMock.Setup(repo => repo.IsLikedShot(_userId, _shotId))
			.ReturnsAsync(true);
		_shotRepoMock.Setup(repo => repo.IsSaved(_userId, _shotId))
			.ReturnsAsync(true);

		// Act
		var result = await _shotServ.GetShotDetail(_userId, _shotId);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.NotNull(jsonResult);
		Assert.Equal(200, jsonResult.StatusCode);
		_shotRepoMock.Verify(repo => repo.GetShotByShotCode(_shotId), Times.Once);
		_shotRepoMock.Verify(repo => repo.IsLikedShot(_userId, _shotId), Times.Once);
		_shotRepoMock.Verify(repo => repo.IsSaved(_userId, _shotId), Times.Once);
	}

	[Fact]
	public async Task GetShotDetailAsync_Should_ReturnSuccess_WhenGettingShotDetailSuccessThoughUserHasNoValue()
	{
		// Arrange
		var shot = new Shot
		{
			Id = _shotId,
			UserId = _userId,
			Html = "<h1>Nothing</h1>",
			Title = "Test",
			View = 0,
			ImageVideos = new List<ImageVideo>
				{ new ImageVideo { Url = "http://image.com", IsMain = true, Type = MediaTypeEnum.Image } },
			Specialties = new List<Specialty> { new Specialty { Name = "Design" } },
			User = new User
			{
				Email = "johndoe@example.com",
				Password = "123",
				Role = RoleEnum.Customer,
				Username = "JohnDoe",
				Slogan = "Test",
				ImageVideos = new List<ImageVideo>
					{ new ImageVideo { Url = "http://user.com", Type = MediaTypeEnum.Image, IsMain = true } }
			}
		};

		_shotRepoMock.Setup(repo => repo.GetShotByShotCode(_shotId))
			.ReturnsAsync(shot);

		// Act
		var result = await _shotServ.GetShotDetail(Guid.Empty, _shotId);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.NotNull(jsonResult);
		Assert.Equal(200, jsonResult.StatusCode);
		_shotRepoMock.Verify(repo => repo.GetShotByShotCode(_shotId), Times.Once);
		_shotRepoMock.Verify(repo => repo.IsLikedShot(_userId, _shotId), Times.Never);
		_shotRepoMock.Verify(repo => repo.IsSaved(_userId, _shotId), Times.Never);
	}

	[Fact]
	public async Task GetShotDetailAsync_Should_ThrowException_WhenGettingUserImageVideosOrSloganAreNull()
	{
		// Arrange
		var shot = new Shot
		{
			Id = _shotId,
			UserId = _userId,
			Html = "<h1>Nothing</h1>",
			Title = "Test",
			View = 0,
			ImageVideos = new List<ImageVideo>
				{ new ImageVideo { Url = "http://image.com", IsMain = true, Type = MediaTypeEnum.Image } },
			Specialties = new List<Specialty> { new Specialty { Name = "Design" } },
			User = new User
			{
				Email = "johndoe@example.com",
				Password = "123",
				Role = RoleEnum.Customer,
				Username = "JohnDoe",
				ImageVideos = new List<ImageVideo> {  }
			}
		};

		_shotRepoMock.Setup(repo => repo.GetShotByShotCode(_shotId))
			.ReturnsAsync(shot);
		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _shotServ.GetShotDetail(_userId, _shotId));
		_shotRepoMock.Verify(repo => repo.GetShotByShotCode(_shotId), Times.Once);
		_shotRepoMock.Verify(repo => repo.IsLikedShot(_userId, _shotId), Times.Never);
		_shotRepoMock.Verify(repo => repo.IsSaved(_userId, _shotId), Times.Never);
	}

	[Fact]
	public async Task GetShotDetailAsync_Should_ThrowException_WhenGettingShotIsNull()
	{
		// Arrange
		_shotRepoMock.Setup(repo => repo.GetShotByShotCode(_shotId))
			.ReturnsAsync((Shot)null);
		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _shotServ.GetShotDetail(_userId, _shotId));
		_shotRepoMock.Verify(repo => repo.GetShotByShotCode(_shotId), Times.Once);
		_shotRepoMock.Verify(repo => repo.IsLikedShot(_userId, _shotId), Times.Never);
		_shotRepoMock.Verify(repo => repo.IsSaved(_userId, _shotId), Times.Never);
	}
}
