using System.Text.Json;
using BE.src.api.domains.DTOs.User;
using BE.src.api.domains.Enum;
using BE.src.api.domains.eventbus.Producers;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Users;
public class EditSocialLinkProfilesServiceTests
{
	private readonly Mock<IUserRepo> _userRepoMock;
	private readonly Mock<IEmailServ> _emailServMock;
	private readonly Mock<ISocialProfileRepo> _socialProfileRepoMock;
	private readonly Mock<IMembershipRepo> _membershipRepoMock;
	private readonly Mock<IPostRepo> _postRepoMock;
	private readonly Mock<INotificationRepo> _notificationRepoMock;
	private readonly Mock<ICacheService> _cacheServiceMock;
	private readonly Mock<IEventBusRabbitMQProducer> _eventBusRabbitMQProducerMock;
	private readonly Guid _userId = Guid.NewGuid();
	private readonly UserEditSocialLinksDTO _userDto = new UserEditSocialLinksDTO
	{
		FacebookLink = "https://facebook.com/test",
		TwitterLink = "https://twitter.com/test"
	};
	private readonly UserServ _userServ;
	public EditSocialLinkProfilesServiceTests()
	{
		_userRepoMock = new Mock<IUserRepo>();
		_emailServMock = new Mock<IEmailServ>();
		_socialProfileRepoMock = new Mock<ISocialProfileRepo>();
		_membershipRepoMock = new Mock<IMembershipRepo>();
		_postRepoMock = new Mock<IPostRepo>();
		_notificationRepoMock = new Mock<INotificationRepo>();
		_cacheServiceMock = new Mock<ICacheService>();
		_eventBusRabbitMQProducerMock = new Mock<IEventBusRabbitMQProducer>();
		_userServ = new UserServ(
			_userRepoMock.Object,
			_emailServMock.Object,
			_socialProfileRepoMock.Object,
			_membershipRepoMock.Object,
			_postRepoMock.Object,
			_notificationRepoMock.Object,
			_cacheServiceMock.Object,
			_eventBusRabbitMQProducerMock.Object);
	}

	[Fact]
	public async Task EditSocialLinkProfilesAsync_Should_ReturnSuccess_UpdateExistingProfiles()
	{
		// Arrange
		var user = new User {
			Id = _userId,
			Username = "TestUser",
			Email = "test@example.com",
			Password = "123",
			Role = RoleEnum.Customer
		};

		var existingProfiles = new List<SocialProfile>
		{
			new SocialProfile { UserId = _userId, Type = TypeSocialEnum.Facebook, Linked = "https://facebook.com/old" },
			new SocialProfile { UserId = _userId, Type = TypeSocialEnum.Twitter, Linked = "https://twitter.com/old" }
		};

		_userRepoMock.Setup(repo => repo.GetUserById(It.IsAny<Guid>(), default))
			.ReturnsAsync(user);
		_socialProfileRepoMock.Setup(repo => repo.GetSocialProfiles(It.IsAny<Guid>()))
			.ReturnsAsync(existingProfiles);
		_socialProfileRepoMock.Setup(repo => repo.AddNewSocialProfile(It.IsAny<SocialProfile>()))
			.ReturnsAsync(true);
		_socialProfileRepoMock.Setup(repo => repo.EditSocialProfile(It.IsAny<SocialProfile>()))
			.ReturnsAsync(true);

		// Act
		var result = await _userServ.EditSocialLinkProfiles(_userId, _userDto);
		var jsonResult = Assert.IsType<JsonResult>(result);
		var json = JsonSerializer.Serialize(jsonResult.Value);
		var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.Equal("Social profiles updated successfully.", response["Message"].ToString().Trim());
		_userRepoMock.Verify(repo => repo.GetUserById(It.IsAny<Guid>(), default), Times.Once);
		_socialProfileRepoMock.Verify(repo => repo.GetSocialProfiles(It.IsAny<Guid>()), Times.Once);
		_socialProfileRepoMock.Verify(repo => repo.EditSocialProfile(It.IsAny<SocialProfile>()), Times.Exactly(2));
		_socialProfileRepoMock.Verify(repo => repo.AddNewSocialProfile(It.IsAny<SocialProfile>()), Times.Once);
	}

	[Fact]
	public async Task EditSocialLinkProfilesAsync_Should_ReturnSuccess_WhenAddingNewProfilesNoneExist()
	{
		// Arrange
		var user = new User
		{
			Id = _userId,
			Username = "TestUser",
			Email = "test@example.com",
			Password = "123",
			Role = RoleEnum.Customer
		};

		_userRepoMock.Setup(repo => repo.GetUserById(It.IsAny<Guid>(), default))
			.ReturnsAsync(user);
		_socialProfileRepoMock.Setup(repo => repo.GetSocialProfiles(It.IsAny<Guid>()))
			.ReturnsAsync(new List<SocialProfile>());
		_socialProfileRepoMock.Setup(repo => repo.AddNewSocialProfile(It.IsAny<SocialProfile>()))
			.ReturnsAsync(true);

		// Act
		var result = await _userServ.EditSocialLinkProfiles(_userId, _userDto);
		var jsonResult = Assert.IsType<JsonResult>(result);
		var json = JsonSerializer.Serialize(jsonResult.Value);
		var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.Equal("Social profiles updated successfully.", response["Message"].ToString().Trim());
		_userRepoMock.Verify(repo => repo.GetUserById(It.IsAny<Guid>(), default), Times.Once);
		_socialProfileRepoMock.Verify(repo => repo.GetSocialProfiles(It.IsAny<Guid>()), Times.Once);
		_socialProfileRepoMock.Verify(repo => repo.EditSocialProfile(It.IsAny<SocialProfile>()), Times.Never);
		_socialProfileRepoMock.Verify(repo => repo.AddNewSocialProfile(It.IsAny<SocialProfile>()), Times.Exactly(3));
	}

	[Fact]
	public async Task EditSocialLinkProfilesAsync_Should_ReturnSuccess_WhenAddingOrUpdatingSomeExist()
	{
		// Arrange
		var user = new User
		{
			Id = _userId,
			Username = "TestUser",
			Email = "test@example.com",
			Password = "123",
			Role = RoleEnum.Customer
		};

		var existingProfiles = new List<SocialProfile>
		{
			new SocialProfile { UserId = _userId, Type = TypeSocialEnum.Facebook, Linked = "https://facebook.com/old" }
		};

		_userRepoMock.Setup(repo => repo.GetUserById(It.IsAny<Guid>(), default))
			.ReturnsAsync(user);
		_socialProfileRepoMock.Setup(repo => repo.GetSocialProfiles(It.IsAny<Guid>()))
			.ReturnsAsync(existingProfiles);
		_socialProfileRepoMock.Setup(repo => repo.EditSocialProfile(It.IsAny<SocialProfile>()))
			.ReturnsAsync(true);
		_socialProfileRepoMock.Setup(repo => repo.AddNewSocialProfile(It.IsAny<SocialProfile>()))
			.ReturnsAsync(true);

		// Act
		var result = await _userServ.EditSocialLinkProfiles(_userId, _userDto);
		var jsonResult = Assert.IsType<JsonResult>(result);
		var json = JsonSerializer.Serialize(jsonResult.Value);
		var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.Equal("Social profiles updated successfully.", response["Message"].ToString().Trim());
		_userRepoMock.Verify(repo => repo.GetUserById(It.IsAny<Guid>(), default), Times.Once);
		_socialProfileRepoMock.Verify(repo => repo.GetSocialProfiles(It.IsAny<Guid>()), Times.Once);
		_socialProfileRepoMock.Verify(repo => repo.EditSocialProfile(It.IsAny<SocialProfile>()), Times.Once);
		_socialProfileRepoMock.Verify(repo => repo.AddNewSocialProfile(It.IsAny<SocialProfile>()), Times.Exactly(2));
	}

	[Fact]
	public async Task EditSocialLinkProfilesAsync_Should_ThrowException_WhenUserNotFound()
	{
		// Arrange
		_userRepoMock.Setup(repo => repo.GetUserById(It.IsAny<Guid>(), default))
			.ReturnsAsync((User)null);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.EditSocialLinkProfiles(_userId, _userDto));
		_userRepoMock.Verify(repo => repo.GetUserById(It.IsAny<Guid>(), default), Times.Once);
		_socialProfileRepoMock.Verify(repo => repo.GetSocialProfiles(It.IsAny<Guid>()), Times.Never);
		_socialProfileRepoMock.Verify(repo => repo.EditSocialProfile(It.IsAny<SocialProfile>()), Times.Never);
		_socialProfileRepoMock.Verify(repo => repo.AddNewSocialProfile(It.IsAny<SocialProfile>()), Times.Never);
	}

	[Fact]
	public async Task EditSocialLinkProfilesAsync_Should_ThrowException_WhenUserRepoThrows()
	{
		// Arrange
		_userRepoMock.Setup(repo => repo.GetUserById(It.IsAny<Guid>(), default))
			.ThrowsAsync(new Exception());

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.EditSocialLinkProfiles(_userId, _userDto));
		_userRepoMock.Verify(repo => repo.GetUserById(It.IsAny<Guid>(), default), Times.Once);
		_socialProfileRepoMock.Verify(repo => repo.GetSocialProfiles(It.IsAny<Guid>()), Times.Never);
		_socialProfileRepoMock.Verify(repo => repo.EditSocialProfile(It.IsAny<SocialProfile>()), Times.Never);
		_socialProfileRepoMock.Verify(repo => repo.AddNewSocialProfile(It.IsAny<SocialProfile>()), Times.Never);
	}
}
