using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Specialties;
public class ViewSpecialtiesServiceTests
{
	private readonly Mock<ISpecialtyRepo> _specialtyMockRepo;
	private readonly Mock<ICacheService> _cacheMockService;
	private readonly SpecialtyServ _specialtyServ;
	public ViewSpecialtiesServiceTests()
	{
		_specialtyMockRepo = new Mock<ISpecialtyRepo>();
		_cacheMockService = new Mock<ICacheService>();
		_specialtyServ = new SpecialtyServ(
			_specialtyMockRepo.Object,
			_cacheMockService.Object);
	}

	[Fact]
	public async Task ViewSpecialtiesAsync_Should_ReturnSuccess_WhenQueryIsNull()
	{
		// Arrange
		var specialties = new List<Specialty>
		{
			new Specialty { Id = Guid.NewGuid(), Name = "Cardiology" },
			new Specialty { Id = Guid.NewGuid(), Name = "Neurology" },
			new Specialty { Id = Guid.NewGuid(), Name = "Orthopedics" },
			new Specialty { Id = Guid.NewGuid(), Name = "Dermatology" },
			new Specialty { Id = Guid.NewGuid(), Name = "Pediatrics" },
			new Specialty { Id = Guid.NewGuid(), Name = "Oncology" },
			new Specialty { Id = Guid.NewGuid(), Name = "Radiology" }
		};

		_specialtyMockRepo.Setup(repo => repo.GetSpecialties())
			.ReturnsAsync(specialties);

		// Act
		var result = await _specialtyServ.ViewSpecialties(null);
		var jsonResult = Assert.IsType<JsonResult>(result);
		var returnedSpecialties = Assert.IsType<List<Specialty>>(jsonResult.Value);

		// Assert
		Assert.Equal(6, returnedSpecialties.Count);
		_specialtyMockRepo.Verify(repo => repo.GetSpecialties(), Times.Once);
		_specialtyMockRepo.Verify(repo => repo.GetSpecialtiesByQuery(It.IsAny<string>()), Times.Never);
	}

	[Fact]
	public async Task ViewSpecialtiesAsync_Should_ReturnSuccess_WhenQueryIsProvided()
	{
		// Arrange
		var query = "Cardiology";
		var specialties = new List<Specialty>
		{
			new Specialty { Id = Guid.NewGuid(), Name = "Cardiology" }
		};

		_specialtyMockRepo.Setup(repo => repo.GetSpecialtiesByQuery(query))
			.ReturnsAsync(specialties);

		// Act
		var result = await _specialtyServ.ViewSpecialties(query);
		var jsonResult = Assert.IsType<JsonResult>(result);
		var returnedSpecialties = Assert.IsType<List<Specialty>>(jsonResult.Value);

		// Assert
		Assert.Single(returnedSpecialties);
		Assert.Equal("Cardiology", returnedSpecialties[0].Name);
		_specialtyMockRepo.Verify(repo => repo.GetSpecialties(), Times.Never);
		_specialtyMockRepo.Verify(repo => repo.GetSpecialtiesByQuery(It.IsAny<string>()), Times.Once);
	}

	[Fact]
	public async Task ViewSpecialtiesAsync_Should_ThrowException_WhenGetSpecialtiesFails()
	{
		// Arrange
		_specialtyMockRepo.Setup(repo => repo.GetSpecialties())
			.ThrowsAsync(new Exception());

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _specialtyServ.ViewSpecialties(null));
		_specialtyMockRepo.Verify(repo => repo.GetSpecialties(), Times.Once);
		_specialtyMockRepo.Verify(repo => repo.GetSpecialtiesByQuery(It.IsAny<string>()), Times.Never);
	}

	[Fact]
	public async Task ViewSpecialties_Should_ThrowException_WhenGetSpecialtiesByQueryFails()
	{
		// Arrange
		var query = "Cardiology";
		_specialtyMockRepo.Setup(repo => repo.GetSpecialtiesByQuery(query))
			.ThrowsAsync(new Exception());

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _specialtyServ.ViewSpecialties(query));
		_specialtyMockRepo.Verify(repo => repo.GetSpecialties(), Times.Never);
		_specialtyMockRepo.Verify(repo => repo.GetSpecialtiesByQuery(It.IsAny<string>()), Times.Once);
	}
}
