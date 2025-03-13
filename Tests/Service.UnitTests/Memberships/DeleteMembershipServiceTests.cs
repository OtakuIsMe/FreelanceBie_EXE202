using BE.src.api.repositories;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Memberships;
public class DeleteMembershipServiceTests
{
	private readonly Mock<IMembershipRepo> _membershipRepoMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly MembershipServ _membershipServ;
	private readonly Guid _membershipId = Guid.NewGuid();
	public DeleteMembershipServiceTests()
	{
		_membershipRepoMock = new Mock<IMembershipRepo>();
		_cacheServiceMock = new Mock<ICacheService>();

		_membershipServ = new MembershipServ(
			_membershipRepoMock.Object,
			_cacheServiceMock.Object);
	}

	[Fact]
	public async Task DeleteMembershipAsync_Should_ReturnSuccess_WhenDeleteMembershipSuccess()
	{
		// Arrange
		_membershipRepoMock.Setup(repo => repo.DeleteMembership(_membershipId))
			.ReturnsAsync(true);

		// Act
		var result = await _membershipServ.DeleteMembership(_membershipId);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.Equal(200, jsonResult.StatusCode);
		_membershipRepoMock.Verify(repo => repo.DeleteMembership(_membershipId), Times.Once);
	}

	[Fact]
	public async Task DeleteMembershipAsync_Should_ThrowException_WhenDeleteMembershipFails()
	{
		// Arrange
		_membershipRepoMock.Setup(repo => repo.DeleteMembership(_membershipId))
			.Throws(new Exception("Database Errors"));

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _membershipServ.DeleteMembership(_membershipId));
		_membershipRepoMock.Verify(repo => repo.DeleteMembership(_membershipId), Times.Once);
	}

	[Fact]
	public async Task DeleteMembershipAsync_Should_ThrowException_WhenMembershipNotFound()
	{
		// Arrange
		_membershipRepoMock.Setup(repo => repo.DeleteMembership(_membershipId))
			.ReturnsAsync(false);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _membershipServ.DeleteMembership(_membershipId));
		_membershipRepoMock.Verify(repo => repo.DeleteMembership(_membershipId), Times.Once);
	}
}
