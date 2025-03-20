using BE.src.api.domains.DTOs.Membership;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Nest;

namespace Service.UnitTests.Memberships;
public class UpdateMembershipServiceTests
{
	private readonly Mock<IMembershipRepo> _membershipRepoMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly MembershipServ _membershipServ;
	private readonly Guid _membershipId = Guid.NewGuid();
	public UpdateMembershipServiceTests()
	{
		_membershipRepoMock = new Mock<IMembershipRepo>();
		_cacheServiceMock = new Mock<ICacheService>();

		_membershipServ = new MembershipServ(
			_membershipRepoMock.Object,
			_cacheServiceMock.Object);
	}

	[Fact]
	public async Task UpdateMembershipAsync_Should_ReturnSuccess_WhenUpdateMembershipSuccess()
	{
		// Arrange
		var membership = new Membership
		{
			Name = "Basic",
			Price = 1000,
			ExpireTime = 3,
			Description = "Basic Package"
		};

		var updateMembership = new MembershipUpdateDTO
		{
			Name = "Standard",
			Price = 2000,
			ExpireTime = 2,
			Description = "Standard Package"
		};

		_membershipRepoMock.Setup(repo => repo.GetMembershipById(_membershipId))
			.ReturnsAsync(membership);
		_membershipRepoMock.Setup(repo => repo.UpdateMembership(It.IsAny<Membership>()))
			.ReturnsAsync(true);
		// Act
		var result = await _membershipServ.UpdateMembership(_membershipId, updateMembership);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.Equal(200, jsonResult.StatusCode);
		_membershipRepoMock.Verify(repo => repo.GetMembershipById(_membershipId), Times.Once);
		_membershipRepoMock.Verify(repo => repo.UpdateMembership(It.IsAny<Membership>()), Times.Once);
	}

	[Fact]
	public async Task UpdateMembershipAsync_Should_ThrowException_WhenMembershipNotFound()
	{
		// Arrange
		var updateMembership = new MembershipUpdateDTO
		{
			Name = "Standard",
			Price = 2000,
			ExpireTime = 2,
			Description = "Standard Package"
		};

		_membershipRepoMock.Setup(repo => repo.GetMembershipById(_membershipId))
			.ReturnsAsync((Membership)null);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() =>
			_membershipServ.UpdateMembership(_membershipId, updateMembership));
		_membershipRepoMock.Verify(repo => repo.GetMembershipById(_membershipId), Times.Once);
		_membershipRepoMock.Verify(repo => repo.UpdateMembership(It.IsAny<Membership>()), Times.Never);
	}

	[Fact]
	public async Task UpdateMembershipAsync_Should_ThrowException_WhenUpdateMembershipFails()
	{
		// Arrange
		var membership = new Membership
		{
			Name = "Basic",
			Price = 1000,
			ExpireTime = 3,
			Description = "Basic Package"
		};

		var updateMembership = new MembershipUpdateDTO
		{
			Name = "Standard",
			Price = 2000,
			ExpireTime = 2,
			Description = "Standard Package"
		};

		_membershipRepoMock.Setup(repo => repo.GetMembershipById(_membershipId))
			.ReturnsAsync(membership);
		_membershipRepoMock.Setup(repo => repo.UpdateMembership(It.IsAny<Membership>()))
			.ReturnsAsync(false);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() =>
			_membershipServ.UpdateMembership(_membershipId, updateMembership));
		_membershipRepoMock.Verify(repo => repo.GetMembershipById(_membershipId), Times.Once);
		_membershipRepoMock.Verify(repo => repo.UpdateMembership(It.IsAny<Membership>()), Times.Once);
	}
}
