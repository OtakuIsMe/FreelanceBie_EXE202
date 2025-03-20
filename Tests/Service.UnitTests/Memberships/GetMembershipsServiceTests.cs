using BE.src.api.domains.DTOs.Membership;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Memberships;
public class GetMembershipsServiceTests
{
	private readonly Mock<IMembershipRepo> _membershipRepoMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly MembershipServ _membershipServ;
	private readonly Guid _membershipId = Guid.NewGuid();
	private readonly string cacheKey = "memberships";
	public GetMembershipsServiceTests()
	{
		_membershipRepoMock = new Mock<IMembershipRepo>();
		_cacheServiceMock = new Mock<ICacheService>();

		_membershipServ = new MembershipServ(
			_membershipRepoMock.Object,
			_cacheServiceMock.Object);
	}

	[Fact]
	public async Task GetMembershipsAsync_Should_ReturnSuccess_WhenGetCacheMemberships()
	{
		// Arrange
		var cacheMemberships = new List<Membership>
		{
			new Membership
			{
				Name = "Basic",
				Price = 1000,
				ExpireTime = 3,
				Description = "Basic Package"
			}
		};

		_cacheServiceMock.Setup(cache => cache.Get<List<Membership>>(cacheKey))
			.ReturnsAsync(cacheMemberships);

		// Act
		var result = await _membershipServ.GetMemberships();

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		var memberships = Assert.IsType<List<Membership>>(jsonResult.Value);
		Assert.Equal(cacheMemberships, memberships);
		_cacheServiceMock.Verify(cache => cache.Get<List<Membership>>(cacheKey), Times.Once);
		_cacheServiceMock.Verify(cache => cache.Set<List<Membership>>(cacheKey, cacheMemberships, TimeSpan.FromMinutes(10)), Times.Never);
	}

	[Fact]
	public async Task GetMembershipsAsync_Should_ReturnSuccess_WhenSetCacheMemberships()
	{
		// Arrange
		var newMemberships = new List<Membership>
		{
			new Membership
			{
				Name = "Basic",
				Price = 1000,
				ExpireTime = 3,
				Description = "Basic Package"
			}
		};

		_cacheServiceMock.Setup(cache => cache.Get<List<Membership>>(cacheKey))
			.ReturnsAsync((List<Membership>)null);
		_membershipRepoMock.Setup(repo => repo.GetMemberships())
			.ReturnsAsync(newMemberships);
		_cacheServiceMock
			.Setup(cache => cache.Set<List<Membership>>(cacheKey, newMemberships, TimeSpan.FromMinutes(10)))
			.Returns(Task.CompletedTask);

		// Act
		var result = await _membershipServ.GetMemberships();

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		var memberships = Assert.IsType<List<Membership>>(jsonResult.Value);
		Assert.Equal(newMemberships, memberships);
		_cacheServiceMock.Verify(cache => cache.Get<List<Membership>>(cacheKey), Times.Once);
		_membershipRepoMock.Verify(repo => repo.GetMemberships(), Times.Once);
		_cacheServiceMock.Verify(cache => cache.Set<List<Membership>>(cacheKey, newMemberships, TimeSpan.FromMinutes(10)), Times.Once);
	}

	[Fact]
	public async Task GetMembershipsAsync_Should_ThrowException_WhenCacheMembershipsNotFound()
	{
		// Arrange
		var newMemberships = new List<Membership>
		{
			new Membership
			{
				Name = "Basic",
				Price = 1000,
				ExpireTime = 3,
				Description = "Basic Package"
			}
		};

		_cacheServiceMock.Setup(cache => cache.Get<List<Membership>>(cacheKey))
			.Throws(new Exception());

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _membershipServ.GetMemberships());
		_cacheServiceMock.Verify(cache => cache.Get<List<Membership>>(cacheKey), Times.Once);
		_membershipRepoMock.Verify(repo => repo.GetMemberships(), Times.Never);
		_cacheServiceMock.Verify(cache => cache.Set<List<Membership>>(cacheKey, newMemberships, TimeSpan.FromMinutes(10)), Times.Never);
	}

	[Fact]
	public async Task GetMembershipsAsync_Should_ThrowException_WhenMembershipsNotFoundAnyValue()
	{
		// Arrange
		var newMemberships = new List<Membership>
		{
			new Membership
			{
				Name = "Basic",
				Price = 1000,
				ExpireTime = 3,
				Description = "Basic Package"
			}
		};

		_cacheServiceMock.Setup(cache => cache.Get<List<Membership>>(cacheKey))
			.ReturnsAsync((List<Membership>)null);
		_membershipRepoMock.Setup(repo => repo.GetMemberships())
			.ReturnsAsync(new List<Membership>());

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _membershipServ.GetMemberships());
		_cacheServiceMock.Verify(cache => cache.Get<List<Membership>>(cacheKey), Times.Once);
		_membershipRepoMock.Verify(repo => repo.GetMemberships(), Times.Once);
		_cacheServiceMock.Verify(cache => cache.Set<List<Membership>>(cacheKey, newMemberships, TimeSpan.FromMinutes(10)), Times.Never);
	}

	[Fact]
	public async Task GetMembershipsAsync_Should_ThrowException_WhenSettingMembershipsCacheKeyFails()
	{
		// Arrange
		var newMemberships = new List<Membership>
		{
			new Membership
			{
				Name = "Basic",
				Price = 1000,
				ExpireTime = 3,
				Description = "Basic Package"
			}
		};

		_cacheServiceMock.Setup(cache => cache.Get<List<Membership>>(cacheKey))
			.ReturnsAsync((List<Membership>)null);
		_membershipRepoMock.Setup(repo => repo.GetMemberships())
			.ReturnsAsync(newMemberships);
		_cacheServiceMock
			.Setup(cache => cache.Set<List<Membership>>(cacheKey, newMemberships, TimeSpan.FromMinutes(10)))
			.Throws(new Exception());

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _membershipServ.GetMemberships());
		_cacheServiceMock.Verify(cache => cache.Get<List<Membership>>(cacheKey), Times.Once);
		_membershipRepoMock.Verify(repo => repo.GetMemberships(), Times.Once);
		_cacheServiceMock.Verify(cache => cache.Set<List<Membership>>(cacheKey, newMemberships, TimeSpan.FromMinutes(10)), Times.Once);
	}
}
