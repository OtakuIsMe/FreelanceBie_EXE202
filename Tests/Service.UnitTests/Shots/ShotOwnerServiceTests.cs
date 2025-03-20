using BE.src.api.domains.DTOs.Shot;
using BE.src.api.domains.Enum;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Shots;

public class ShotOwnerServiceTests
{
	private readonly Mock<IShotRepo> _shotRepoMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly Mock<ISpecialtyRepo> _specialtyRepoMock;
	private readonly ShotServ _shotServ;
	private readonly Guid _userId = Guid.NewGuid();
	private readonly Guid _shotId = Guid.NewGuid();
	public ShotOwnerServiceTests()
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
	public async Task ShotOwerAsync_Should_ReturnSuccess_WhenShotOwerSuccess()
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
				View = 0,
				ImageVideos = new List<ImageVideo> { new ImageVideo { Url = "http://image.com", IsMain = true, Type = MediaTypeEnum.Image} },
				Specialties = new List<Specialty> { new Specialty { Name = "Design" } },
				User = new User
				{
					Email = "johndoe@example.com",
					Password = "123",
					Role = RoleEnum.Customer,
					Username = "JohnDoe",
					ImageVideos = new List<ImageVideo> { new ImageVideo { Url = "http://user.com", Type = MediaTypeEnum.Image, IsMain = true } }
				}
			}
		};

		_shotRepoMock.Setup(repo => repo.GetShotsByUser(_userId))
			.ReturnsAsync(shots);
		_shotRepoMock.Setup(repo => repo.GetLikeCount(It.IsAny<Guid>()))
			.ReturnsAsync(1);
		_shotRepoMock.Setup(repo => repo.GetViewCount(It.IsAny<Guid>()))
			.ReturnsAsync(1);

		// Act
		var result = await _shotServ.ShotOwner(_userId);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.Equal(200, jsonResult.StatusCode);
		_shotRepoMock.Verify(repo => repo.GetShotsByUser(_userId), Times.Once);
		_shotRepoMock.Verify(repo => repo.GetLikeCount(It.IsAny<Guid>()), Times.AtLeast(1));
		_shotRepoMock.Verify(repo => repo.GetViewCount(It.IsAny<Guid>()), Times.AtLeast(1));
	}

	[Fact]
	public async Task ShotOwerAsync_Should_ThrowException_WhenGettingShotByUserFails()
	{
		// Arrange
		_shotRepoMock.Setup(repo => repo.GetShotsByUser(_userId)).Throws(new Exception());
		_shotRepoMock.Setup(repo => repo.GetLikeCount(It.IsAny<Guid>())).ReturnsAsync(1);
		_shotRepoMock.Setup(repo => repo.GetViewCount(It.IsAny<Guid>())).ReturnsAsync(1);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _shotServ.ShotOwner(_userId));
		_shotRepoMock.Verify(repo => repo.GetShotsByUser(_userId), Times.Once);
		_shotRepoMock.Verify(repo => repo.GetLikeCount(It.IsAny<Guid>()), Times.Never);
		_shotRepoMock.Verify(repo => repo.GetViewCount(It.IsAny<Guid>()), Times.Never);
	}

	[Fact]
	public async Task ShotOwerAsync_Should_ThrowException_WhenRequiredPropertiesCannotBeNull()
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
				View = 0,
				Specialties = new List<Specialty> { new Specialty { Name = "Design" } },
				User = new User
				{
					Email = "johndoe@example.com",
					Password = "123",
					Role = RoleEnum.Customer,
					Username = "JohnDoe",
				}
			}
		};

		_shotRepoMock.Setup(repo => repo.GetShotsByUser(_userId))
			.ReturnsAsync(shots);
		_shotRepoMock.Setup(repo => repo.GetLikeCount(It.IsAny<Guid>()))
			.ReturnsAsync(1);
		_shotRepoMock.Setup(repo => repo.GetViewCount(It.IsAny<Guid>()))
			.ReturnsAsync(1);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _shotServ.ShotOwner(_userId));
		_shotRepoMock.Verify(repo => repo.GetShotsByUser(_userId), Times.Once);
		_shotRepoMock.Verify(repo => repo.GetLikeCount(It.IsAny<Guid>()), Times.AtLeast(1));
		_shotRepoMock.Verify(repo => repo.GetViewCount(It.IsAny<Guid>()), Times.AtLeast(1));
	}

	[Fact]
	public async Task ShotOwerAsync_Should_ThrowException_WhenGettingShotOwnerFails()
	{
		// Arrange
		_shotRepoMock.Setup(repo => repo.GetShotsByUser(Guid.Empty)).ReturnsAsync(new List<Shot>());
		_shotRepoMock.Setup(repo => repo.GetLikeCount(It.IsAny<Guid>())).ReturnsAsync(1);
		_shotRepoMock.Setup(repo => repo.GetViewCount(It.IsAny<Guid>())).ReturnsAsync(1);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _shotServ.ShotOwner(_userId));
		_shotRepoMock.Verify(repo => repo.GetShotsByUser(_userId), Times.Once);
		_shotRepoMock.Verify(repo => repo.GetLikeCount(It.IsAny<Guid>()), Times.Never);
		_shotRepoMock.Verify(repo => repo.GetViewCount(It.IsAny<Guid>()), Times.Never);
	}
}
