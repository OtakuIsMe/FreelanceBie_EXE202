using BE.src.api.domains.DTOs.Specialty;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Specialties;
public class UpdateSpecialtyServiceTests
{
	private readonly Mock<ISpecialtyRepo> _specialtyMockRepo;
	private readonly Mock<ICacheService> _cacheMockService;
	private readonly SpecialtyServ _specialtyServ;
	private readonly Guid _specialtyId = Guid.NewGuid();
	public UpdateSpecialtyServiceTests()
	{
		_specialtyMockRepo = new Mock<ISpecialtyRepo>();
		_cacheMockService = new Mock<ICacheService>();
		_specialtyServ = new SpecialtyServ(
			_specialtyMockRepo.Object,
			_cacheMockService.Object);
	}

	[Fact]
	public async Task UpdateSpecialtyAsync_Should_ReturnSuccess_WhenSpecialtyIsUpdatedSuccessfully()
	{
		// Arrange
		var specialtyDto = new SpecialtyCreateDTO { Name = "Updated Name" };
		var specialty = new Specialty { Id = _specialtyId, Name = "Old Name" };

		_specialtyMockRepo.Setup(repo => repo.GetSpecialty(It.IsAny<Guid>())).ReturnsAsync(specialty);
		_specialtyMockRepo.Setup(repo => repo.UpdateSpecialty(It.IsAny<Specialty>())).ReturnsAsync(true);

		// Act
		var result = await _specialtyServ.UpdateSpecialty(_specialtyId, specialtyDto);
		var jsonResult = Assert.IsType<JsonResult>(result);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		_specialtyMockRepo.Verify(repo => repo.GetSpecialty(It.IsAny<Guid>()), Times.Once);
		_specialtyMockRepo.Verify(repo => repo.UpdateSpecialty(It.IsAny<Specialty>()), Times.Once);
	}

	[Fact]
	public async Task UpdateSpecialtyAsync_Should_ThrowException_WhenSpecialtyNotFound()
	{
		// Arrange
		var specialtyDto = new SpecialtyCreateDTO { Name = "Updated Name" };
		_specialtyMockRepo.Setup(repo => repo.GetSpecialty(It.IsAny<Guid>())).ReturnsAsync((Specialty)null);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _specialtyServ.UpdateSpecialty(_specialtyId, specialtyDto));
		_specialtyMockRepo.Verify(repo => repo.GetSpecialty(It.IsAny<Guid>()), Times.Once);
		_specialtyMockRepo.Verify(repo => repo.UpdateSpecialty(It.IsAny<Specialty>()), Times.Never);
	}

	[Fact]
	public async Task UpdateSpecialtyAsync_Should_ThrowException_WhenUpdateFails()
	{
		// Arrange
		var specialtyDto = new SpecialtyCreateDTO { Name = "Updated Name" };
		var specialty = new Specialty { Id = _specialtyId, Name = "Old Name" };

		_specialtyMockRepo.Setup(repo => repo.GetSpecialty(It.IsAny<Guid>())).ReturnsAsync(specialty);
		_specialtyMockRepo.Setup(repo => repo.UpdateSpecialty(It.IsAny<Specialty>())).ReturnsAsync(false);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _specialtyServ.UpdateSpecialty(_specialtyId, specialtyDto));
		_specialtyMockRepo.Verify(repo => repo.GetSpecialty(It.IsAny<Guid>()), Times.Once);
		_specialtyMockRepo.Verify(repo => repo.UpdateSpecialty(It.IsAny<Specialty>()), Times.Once);
	}

	[Fact]
	public async Task UpdateSpecialtyAsync_Should_ThrowException_WhenRepositoryThrowsException()
	{
		// Arrange
		var specialtyDto = new SpecialtyCreateDTO { Name = "Updated Name" };

		_specialtyMockRepo.Setup(repo => repo.GetSpecialty(It.IsAny<Guid>())).ThrowsAsync(new Exception());

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _specialtyServ.UpdateSpecialty(_specialtyId, specialtyDto));
		_specialtyMockRepo.Verify(repo => repo.GetSpecialty(It.IsAny<Guid>()), Times.Once);
		_specialtyMockRepo.Verify(repo => repo.UpdateSpecialty(It.IsAny<Specialty>()), Times.Never);
	}
}
