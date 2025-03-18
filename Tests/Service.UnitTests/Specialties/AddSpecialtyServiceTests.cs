using BE.src.api.domains.DTOs.Specialty;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Specialties;
public class AddSpecialtyServiceTests
{
	private readonly Mock<ISpecialtyRepo> _specialtyMockRepo;
	private readonly Mock<ICacheService> _cacheMockService;
	private readonly SpecialtyServ _specialtyServ;
	public AddSpecialtyServiceTests()
	{
		_specialtyMockRepo = new Mock<ISpecialtyRepo>();
		_cacheMockService = new Mock<ICacheService>();
		_specialtyServ = new SpecialtyServ(
			_specialtyMockRepo.Object,
			_cacheMockService.Object);
	}

	[Fact]
	public async Task AddSpecialtyAsync_Should_ReturnSuccess_WhenSpecialtyIsAddedSuccessfully()
	{
		// Arrange
		var specialtyDto = new SpecialtyCreateDTO { Name = "Cardiology" };
		_specialtyMockRepo.Setup(repo => repo.AddSpecialty(It.IsAny<Specialty>())).ReturnsAsync(true);
		_cacheMockService.Setup(cache => cache.ClearWithPattern("specialties")).Returns(Task.CompletedTask);

		// Act
		var result = await _specialtyServ.AddSpecialty(specialtyDto);
		var jsonResult = Assert.IsType<JsonResult>(result);

		// Assert
		Assert.Equal(201, jsonResult.StatusCode);
		_specialtyMockRepo.Verify(repo => repo.AddSpecialty(It.IsAny<Specialty>()), Times.Once);
		_cacheMockService.Verify(cache => cache.ClearWithPattern("specialties"), Times.Once);
	}

	[Fact]
	public async Task AddSpecialtyAsync_Should_ThrowException_WhenAddingSpecialtyFails()
	{
		// Arrange
		var specialtyDto = new SpecialtyCreateDTO { Name = "Cardiology" };
		_specialtyMockRepo.Setup(repo => repo.AddSpecialty(It.IsAny<Specialty>())).ReturnsAsync(false);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _specialtyServ.AddSpecialty(specialtyDto));
		_specialtyMockRepo.Verify(repo => repo.AddSpecialty(It.IsAny<Specialty>()), Times.Once);
		_cacheMockService.Verify(cache => cache.ClearWithPattern("specialties"), Times.Never);
	}

	[Fact]
	public async Task AddSpecialtyAsync_Should_ThrowException_WhenRepositoryThrowsException()
	{
		// Arrange
		var specialtyDto = new SpecialtyCreateDTO { Name = "Cardiology" };
		_specialtyMockRepo.Setup(repo => repo.AddSpecialty(It.IsAny<Specialty>())).ThrowsAsync(new Exception());

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _specialtyServ.AddSpecialty(specialtyDto));
		_specialtyMockRepo.Verify(repo => repo.AddSpecialty(It.IsAny<Specialty>()), Times.Once);
		_cacheMockService.Verify(cache => cache.ClearWithPattern("specialties"), Times.Never);
	}

	[Fact]
	public async Task AddSpecialtyAsync_Should_ThrowException_WhenClearingPatternsCacheKeyFails()
	{
		// Arrange
		var specialtyDto = new SpecialtyCreateDTO { Name = "Cardiology" };
		_specialtyMockRepo.Setup(repo => repo.AddSpecialty(It.IsAny<Specialty>())).ReturnsAsync(true);
		_cacheMockService.Setup(cache => cache.ClearWithPattern("specialties")).ThrowsAsync(new Exception());

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _specialtyServ.AddSpecialty(specialtyDto));
		_specialtyMockRepo.Verify(repo => repo.AddSpecialty(It.IsAny<Specialty>()), Times.Once);
		_cacheMockService.Verify(cache => cache.ClearWithPattern("specialties"), Times.Once);
	}
}
