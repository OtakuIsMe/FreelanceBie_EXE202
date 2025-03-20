using BE.src.api.domains.DTOs.Shot;
using BE.src.api.domains.Enum;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Shots;

public class GetShotsServiceTests
{
	private readonly Mock<IShotRepo> _shotRepoMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly Mock<ISpecialtyRepo> _specialtyRepoMock;
	private readonly ShotServ _shotServ;
	private readonly Guid _userId = Guid.NewGuid();
	private readonly Guid _shotId = Guid.NewGuid();

	public GetShotsServiceTests()
	{
		string envPath = Path.GetFullPath(Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory, "../../../../../BE/.env"));

		Env.Load(envPath);

		_shotRepoMock = new Mock<IShotRepo>();
		_cacheServiceMock = new Mock<ICacheService>();
		_specialtyRepoMock = new Mock<ISpecialtyRepo>();
		_shotServ = new ShotServ(
			_shotRepoMock.Object,
			_cacheServiceMock.Object,
			_specialtyRepoMock.Object);
	}

	[Fact]
	public async Task GetShotsAsync_Should_ReturnSuccess_WhenGettingCacheKeySuccess()
	{
		// Arrange
		var shotFilter = new ShotSearchFilterDTO
		{
			UserEmail = "user@example.com"
		};

		var shots = new List<Shot>
		{
			new Shot
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
					Email = "user@example.com",
					Password = "123",
					Role = RoleEnum.Customer,
					Username = "JohnDoe",
					Slogan = "Test",
					ImageVideos = new List<ImageVideo>
						{ new ImageVideo { Url = "http://user.com", Type = MediaTypeEnum.Image, IsMain = true } }
				}
			}
		};

		_cacheServiceMock.Setup(cache => cache.Get<List<Shot>>(It.IsAny<string>()))
			.ReturnsAsync(shots);

		// Act
		var result = await _shotServ.GetShots(shotFilter);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.Equal(200, jsonResult.StatusCode);
		_cacheServiceMock.Verify(cache => cache.Get<List<Shot>>(It.IsAny<string>()), Times.Once);
		_shotRepoMock.Verify(repo => repo.GetShots(shotFilter), Times.Never);
		_cacheServiceMock.Verify(cache => cache.Set(It.IsAny<string>(), shots, TimeSpan.FromMinutes(10)), Times.Never);
	}

	[Fact]
	public async Task GetShotsAsync_Should_ReturnSuccess_WhenGettingDataFromDatabaseSuccess()
	{
		// Arrange
		var shotFilter = new ShotSearchFilterDTO
		{
			UserEmail = "user@example.com"
		};

		var shots = new List<Shot>
		{
			new Shot
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
					Email = "user@example.com",
					Password = "123",
					Role = RoleEnum.Customer,
					Username = "JohnDoe",
					Slogan = "Test",
					ImageVideos = new List<ImageVideo>
						{ new ImageVideo { Url = "http://user.com", Type = MediaTypeEnum.Image, IsMain = true } }
				}
			}
		};

		_cacheServiceMock.Setup(cache => cache.Get<List<Shot>>(String.Empty))
			.ReturnsAsync(new List<Shot>());
		_shotRepoMock.Setup(repo => repo.GetShots(shotFilter))
			.ReturnsAsync(shots);
		_cacheServiceMock.Setup(cache => cache.Set(It.IsAny<string>(), shots, TimeSpan.FromMinutes(10)))
			.Returns(Task.CompletedTask);

		// Act
		var result = await _shotServ.GetShots(shotFilter);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.Equal(200, jsonResult.StatusCode);
		_cacheServiceMock.Verify(cache => cache.Get<List<Shot>>(It.IsAny<string>()), Times.Once);
		_shotRepoMock.Verify(repo => repo.GetShots(shotFilter), Times.Once);
		_cacheServiceMock.Verify(cache => cache.Set(It.IsAny<string>(), shots, TimeSpan.FromMinutes(10)), Times.Once);
	}

	[Fact]
	public async Task GetShotsAsync_Should_ThrowException_WhenGettingDataFromDBAreNotFound()
	{
		// Arrange
		var shotFilter = new ShotSearchFilterDTO
		{
			UserEmail = "test"
		};

		var shots = new List<Shot> { };

		_cacheServiceMock.Setup(cache => cache.Get<List<Shot>>(String.Empty))
			.ReturnsAsync(new List<Shot>());
		_shotRepoMock.Setup(repo => repo.GetShots(shotFilter))
			.ReturnsAsync(shots);
		_cacheServiceMock.Setup(cache => cache.Set(It.IsAny<string>(), shots, TimeSpan.FromMinutes(10)))
			.Returns(Task.CompletedTask);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _shotServ.GetShots(shotFilter));
		_cacheServiceMock.Verify(cache => cache.Get<List<Shot>>(It.IsAny<string>()), Times.Once);
		_shotRepoMock.Verify(repo => repo.GetShots(shotFilter), Times.Once);
		_cacheServiceMock.Verify(cache => cache.Set(It.IsAny<string>(), shots, TimeSpan.FromMinutes(10)), Times.Never);
	}
}
