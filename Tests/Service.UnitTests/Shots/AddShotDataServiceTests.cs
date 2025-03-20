using System.Text;
using BE.src.api.domains.DTOs.Shot;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using DotNetEnv;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Shots;
public class AddShotDataServiceTests
{
	private readonly Mock<IShotRepo> _shotRepoMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly Mock<ISpecialtyRepo> _specialtyRepoMock;
	private readonly ShotServ _shotServ;
	private readonly Guid _userId = Guid.NewGuid();
    public AddShotDataServiceTests()
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
    public async Task AddShotDataAsync_Should_ReturnSuccess_WhenPostIsCreated()
    {
		// Arrange
		var fileMock = new Mock<IFormFile>();
		var content = "Fake file content";
		var fileName = "test.png";
		var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
		fileMock.Setup(_ => _.OpenReadStream()).Returns(stream);
		fileMock.Setup(_ => _.FileName).Returns(fileName);
		fileMock.Setup(_ => _.Length).Returns(stream.Length);

		var request = new ShotAddData
		{
			Title = "Test Shot",
			Html = "<p>Sample HTML</p>",
			Specialties = new List<string> { "Photography" },
			Images = new List<FileShotAdd> { new FileShotAdd { Replace = "old_url", File = fileMock.Object }}
		};

		_specialtyRepoMock.Setup(repo => repo.GetSpecialtyByName(It.IsAny<string>())).ReturnsAsync((Specialty)null);
		_specialtyRepoMock.Setup(repo => repo.AddSpecialty(It.IsAny<Specialty>())).ReturnsAsync(true);
		_shotRepoMock.Setup(repo => repo.CreateShot(It.IsAny<Shot>())).ReturnsAsync(true);
		_cacheServiceMock.Setup(cache => cache.ClearWithPattern("shots")).Returns(Task.CompletedTask);

		// Act
		var result = await _shotServ.AddShotData(request, _userId);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.Equal(201, jsonResult.StatusCode);
		_specialtyRepoMock.Verify(repo => repo.GetSpecialtyByName(It.IsAny<string>()), Times.Once);
		_specialtyRepoMock.Verify(repo => repo.AddSpecialty(It.IsAny<Specialty>()), Times.Once);
		_shotRepoMock.Verify(repo => repo.CreateShot(It.IsAny<Shot>()), Times.Once());
		_cacheServiceMock.Verify(cache => cache.ClearWithPattern("shots"), Times.Once);
    }

    [Fact]
    public async Task AddShotDataAsync_Should_ThrowException_WhenCannotCreateShot()
    {
		// Arrange
		var request = new ShotAddData
		{
			Title = "Test",
			Html = "<p>Test</p>",
			Specialties = new List<string> { "Photography" },
			Images = new List<FileShotAdd>()
		};

		var specialty = new Specialty { Id = Guid.NewGuid(), Name = "Photography" };

		_specialtyRepoMock.Setup(repo => repo.GetSpecialtyByName("Photography")).ReturnsAsync(specialty);
		_shotRepoMock.Setup(repo => repo.CreateShot(It.IsAny<Shot>())).ReturnsAsync(false);

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _shotServ.AddShotData(request, _userId));
		_specialtyRepoMock.Verify(repo => repo.GetSpecialtyByName(It.IsAny<string>()), Times.Once);
	    _specialtyRepoMock.Verify(repo => repo.AddSpecialty(It.IsAny<Specialty>()), Times.Never);
	    _shotRepoMock.Verify(repo => repo.CreateShot(It.IsAny<Shot>()), Times.Once());
	    _cacheServiceMock.Verify(cache => cache.ClearWithPattern("shots"), Times.Never);
    }

    [Fact]
    public async Task AddShotDataAsync_Should_ThrowException_WhenDatabaseGetException()
    {
		// Arrange
		var request = new ShotAddData
		{
			Title = "Test",
			Html = "<p>Test</p>",
			Specialties = new List<string> { "Photography" },
			Images = new List<FileShotAdd>()
		};

		var specialty = new Specialty { Id = Guid.NewGuid(), Name = "Photography" };

		_specialtyRepoMock.Setup(repo => repo.GetSpecialtyByName("Photography")).ReturnsAsync(specialty);
		_shotRepoMock.Setup(repo => repo.CreateShot(It.IsAny<Shot>())).ThrowsAsync(new Exception("DB error"));

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _shotServ.AddShotData(request, _userId));
		_specialtyRepoMock.Verify(repo => repo.GetSpecialtyByName(It.IsAny<string>()), Times.Once);
	    _specialtyRepoMock.Verify(repo => repo.AddSpecialty(It.IsAny<Specialty>()), Times.Never);
	    _shotRepoMock.Verify(repo => repo.CreateShot(It.IsAny<Shot>()), Times.Once());
	    _cacheServiceMock.Verify(cache => cache.ClearWithPattern("shots"), Times.Never);
    }
}
