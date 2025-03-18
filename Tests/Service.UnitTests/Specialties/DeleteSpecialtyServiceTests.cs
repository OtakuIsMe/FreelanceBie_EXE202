using BE.src.api.repositories;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Specialties;
public class DeleteSpecialtyServiceTests
{
	private readonly Mock<ISpecialtyRepo> _specialtyMockRepo;
	private readonly Mock<ICacheService> _cacheMockService;
	private readonly SpecialtyServ _specialtyServ;
	private readonly Guid _specialtyId = Guid.NewGuid();
	public DeleteSpecialtyServiceTests()
	{
		_specialtyMockRepo = new Mock<ISpecialtyRepo>();
		_cacheMockService = new Mock<ICacheService>();
		_specialtyServ = new SpecialtyServ(
			_specialtyMockRepo.Object,
			_cacheMockService.Object);
	}

	[Fact]
	public async Task DeleteSpecialtyAsync_Should_ReturnSuccess_WhenSpecialtyIsDeletedSuccessfully()
	{
		// Arrange
		_specialtyMockRepo.Setup(repo => repo.DeleteSpecialty(It.IsAny<Guid>())).ReturnsAsync(true);

		// Act
		var result = await _specialtyServ.DeleteSpecialty(_specialtyId);
		var jsonResult = Assert.IsType<JsonResult>(result);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		_specialtyMockRepo.Verify(repo => repo.DeleteSpecialty(_specialtyId), Times.Once);
	}

	[Fact]
	public async Task DeleteSpecialtyAsync_Should_ThrowException_WhenDeletionFails()
	{
		// Arrange
		_specialtyMockRepo.Setup(repo => repo.DeleteSpecialty(It.IsAny<Guid>())).ReturnsAsync(false);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _specialtyServ.DeleteSpecialty(_specialtyId));
		_specialtyMockRepo.Verify(repo => repo.DeleteSpecialty(_specialtyId), Times.Once);
	}

	[Fact]
	public async Task DeleteSpecialtyAsync_Should_ThrowException_WhenRepositoryThrowsException()
	{
		// Arrange
		_specialtyMockRepo.Setup(repo => repo.DeleteSpecialty(It.IsAny<Guid>())).ThrowsAsync(new Exception());

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _specialtyServ.DeleteSpecialty(_specialtyId));
		_specialtyMockRepo.Verify(repo => repo.DeleteSpecialty(_specialtyId), Times.Once);
	}
}
