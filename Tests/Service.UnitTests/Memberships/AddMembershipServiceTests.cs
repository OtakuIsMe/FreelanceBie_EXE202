using BE.src.api.domains.DTOs.Membership;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Memberships;
public class AddMembershipServiceTests
{
	private readonly Mock<IMembershipRepo> _membershipRepoMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly MembershipServ _membershipServ;
	public AddMembershipServiceTests()
	{
		_membershipRepoMock = new Mock<IMembershipRepo>();
		_cacheServiceMock = new Mock<ICacheService>();

		_membershipServ = new MembershipServ(
			_membershipRepoMock.Object,
			_cacheServiceMock.Object);
	}

	[Fact]
	public async Task CreateMembershipAsync_Should_ReturnSuccess_WhenCreateMembershipSuccess()
	{
		// Arrange
		var newMembership = new MembershipCreateDTO
		{
			Name = "Basic",
			Price = 1000,
			ExpireTime = 3,
			Description = "Basic Package"
		};

		_membershipRepoMock.Setup(repo => repo.CreateMembership(It.IsAny<Membership>()))
			.ReturnsAsync(true);

		// Act
		var result = await _membershipServ.CreateMembership(newMembership);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.Equal(201, jsonResult.StatusCode);
		_membershipRepoMock.Verify(repo => repo.CreateMembership(It.IsAny<Membership>()), Times.Once);
	}

	[Fact]
	public async Task CreateMembershipAsync_Should_ThrowException_WhenCreateMembershipFails()
	{
		// Arrange
		var newMembership = new MembershipCreateDTO
		{
			Name = "Basic",
			Price = 1000,
			ExpireTime = 3,
			Description = "Basic Package"
		};

		_membershipRepoMock.Setup(repo => repo.CreateMembership(It.IsAny<Membership>()))
			.ReturnsAsync(false);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _membershipServ.CreateMembership(newMembership));
		_membershipRepoMock.Verify(repo => repo.CreateMembership(It.IsAny<Membership>()), Times.Once);
	}
}
