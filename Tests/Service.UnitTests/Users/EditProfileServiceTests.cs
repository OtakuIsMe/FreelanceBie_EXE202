using System.Text.Json;
using BE.src.api.domains.DTOs.User;
using BE.src.api.domains.Enum;
using BE.src.api.domains.eventbus.Producers;
using BE.src.api.domains.Model;
using BE.src.api.helpers;
using BE.src.api.repositories;
using BE.src.api.services;
using DotNetEnv;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.Users;
public class EditProfileServiceTests
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
	private readonly UserEditProfileDTO _userDto = new UserEditProfileDTO
	{
		Name = "New Name",
		Phone = "123456789",
		City = "New City",
		Education = "New Education",
		Description = "New Description",
		DOB = "2000-01-01",
	};
	private readonly UserServ _userServ;
	public EditProfileServiceTests()
	{
		string envPath = Path.GetFullPath(Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory, "../../../../../BE/.env"));
		Env.Load(envPath);
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

	private IFormFile CreateFakeFormFile(string fileName, string contentType)
	{
		var stream = new MemoryStream();
		var writer = new StreamWriter(stream);
		writer.Write("fake image data");
		writer.Flush();
		stream.Position = 0;

		return new FormFile(stream, 0, stream.Length, "file", fileName)
		{
			Headers = new HeaderDictionary(),
			ContentType = contentType
		};
	}
	private User CreateFakeUser()
	{
		return new User
		{
			Id = _userId,
			Name = "Old Name",
			Phone = "987654321",
			City = "Old City",
			Email = "user@example.com",
			Password = "123",
			Role = RoleEnum.Customer,
			Username = "User Name",
		};
	}

	[Fact]
	public async Task EditProfileAsync_Should_ReturnSuccess_WhenUpdatingProfileSuccess()
	{
		// Arrange
		var user = CreateFakeUser();

		_userRepoMock.Setup(repo => repo.GetUserById(It.IsAny<Guid>(), default))
			.ReturnsAsync(user);
		_userRepoMock.Setup(repo => repo.EditProfile(It.IsAny<User>()))
			.ReturnsAsync(true);

		// Act
		var result = await _userServ.EditProfile(_userId, _userDto);
		var jsonResult = Assert.IsType<JsonResult>(result);
		var json = JsonSerializer.Serialize(jsonResult.Value);
		var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.Equal("Profile updated successfully.", response["Message"].ToString().Trim());
		_userRepoMock.Verify(repo => repo.GetUserById(It.IsAny<Guid>(), default), Times.Once);
		_userRepoMock.Verify(repo => repo.AddImageVideo(It.IsAny<ImageVideo>()), Times.Never);
		_userRepoMock.Verify(repo => repo.UpdateImageVideo(It.IsAny<ImageVideo>()), Times.Never);
		_userRepoMock.Verify(repo => repo.EditProfile(It.IsAny<User>()), Times.Once);
	}

	[Fact]
	public async Task EditProfileAsync_Should_ReturnSuccess_WhenDataIsNull()
	{
		// Arrange
		var user = CreateFakeUser();

		_userDto.Image = CreateFakeFormFile("avatar.png", "image/png");
		var emptyDto = new UserEditProfileDTO();

		_userRepoMock.Setup(repo => repo.GetUserById(It.IsAny<Guid>(), default))
			.ReturnsAsync(user);
		_userRepoMock.Setup(repo => repo.EditProfile(It.IsAny<User>()))
			.ReturnsAsync(true);

		// Act
		var result = await _userServ.EditProfile(_userId, emptyDto);
		var jsonResult = Assert.IsType<JsonResult>(result);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		_userRepoMock.Verify(repo => repo.GetUserById(It.IsAny<Guid>(), default), Times.Once);
		_userRepoMock.Verify(repo => repo.AddImageVideo(It.IsAny<ImageVideo>()), Times.Never);
		_userRepoMock.Verify(repo => repo.UpdateImageVideo(It.IsAny<ImageVideo>()), Times.Never);
		_userRepoMock.Verify(repo => repo.EditProfile(It.IsAny<User>()), Times.Once);
	}

	[Fact]
	public async Task EditProfileAsync_Should_ReturnSuccess_WhenUpdatingUserHasNoImageSuccess()
	{
		// Arrange
		var user = CreateFakeUser();
		_userDto.Image = CreateFakeFormFile("avatar.png", "image/png");

		_userRepoMock.Setup(repo => repo.GetUserById(It.IsAny<Guid>(), default))
			.ReturnsAsync(user);
		_userRepoMock.Setup(repo => repo.AddImageVideo(It.IsAny<ImageVideo>()))
			.ReturnsAsync(true);
		_userRepoMock.Setup(repo => repo.EditProfile(It.IsAny<User>())).
			ReturnsAsync(true);

		// Act
		var result = await _userServ.EditProfile(_userId, _userDto);
		var jsonResult = Assert.IsType<JsonResult>(result);
		var json = JsonSerializer.Serialize(jsonResult.Value);
		var response = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.Equal("Profile updated successfully.", response["Message"].ToString().Trim());
		_userRepoMock.Verify(repo => repo.GetUserById(It.IsAny<Guid>(), default), Times.Once);
		_userRepoMock.Verify(repo => repo.AddImageVideo(It.IsAny<ImageVideo>()), Times.Once);
		_userRepoMock.Verify(repo => repo.UpdateImageVideo(It.IsAny<ImageVideo>()), Times.Never);
		_userRepoMock.Verify(repo => repo.EditProfile(It.IsAny<User>()), Times.Once);
	}

	[Fact]
	public async Task EditProfileAsync_Should_ReturnSuccess_WhenUserHasExistingImage()
	{
		// Arrange
		var user = CreateFakeUser();
		user.ImageVideos = new List<ImageVideo>
		{
			new ImageVideo { Url = "https://user_image.com", IsMain = true, Type = MediaTypeEnum.Image }
		};
		_userDto.Image = CreateFakeFormFile("avatar.png", "image/png");

		_userRepoMock.Setup(repo => repo.GetUserById(It.IsAny<Guid>(), default))
			.ReturnsAsync(user);
		_userRepoMock.Setup(repo => repo.UpdateImageVideo(It.IsAny<ImageVideo>()))
			.ReturnsAsync(true);
		_userRepoMock.Setup(repo => repo.EditProfile(It.IsAny<User>())).
			ReturnsAsync(true);

		// Act
		var result = await _userServ.EditProfile(_userId, _userDto);
		var jsonResult = Assert.IsType<JsonResult>(result);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		_userRepoMock.Verify(repo => repo.GetUserById(It.IsAny<Guid>(), default), Times.Once);
		_userRepoMock.Verify(repo => repo.AddImageVideo(It.IsAny<ImageVideo>()), Times.Never);
		_userRepoMock.Verify(repo => repo.UpdateImageVideo(It.IsAny<ImageVideo>()), Times.Once);
		_userRepoMock.Verify(repo => repo.EditProfile(It.IsAny<User>()), Times.Once);
	}

	[Fact]
	public async Task EditProfileAsync_Should_ReturnSuccess_WhenUpdatingDOBIsValid()
	{
		// Arrange
		var user = CreateFakeUser();

		_userRepoMock.Setup(repo => repo.GetUserById(It.IsAny<Guid>(), default))
			.ReturnsAsync(user);
		_userRepoMock.Setup(repo => repo.EditProfile(It.IsAny<User>()))
			.ReturnsAsync(true);

		// Act
		var result = await _userServ.EditProfile(_userId, _userDto);
		var jsonResult = Assert.IsType<JsonResult>(result);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.Equal(DateOnly.Parse("2000-01-01"), user.DOB);
		_userRepoMock.Verify(repo => repo.GetUserById(It.IsAny<Guid>(), default), Times.Once);
		_userRepoMock.Verify(repo => repo.AddImageVideo(It.IsAny<ImageVideo>()), Times.Never);
		_userRepoMock.Verify(repo => repo.UpdateImageVideo(It.IsAny<ImageVideo>()), Times.Never);
		_userRepoMock.Verify(repo => repo.EditProfile(It.IsAny<User>()), Times.Once);
	}

	[Fact]
	public async Task EditProfileAsync_Should_ReturnSuccess_WhenIgnoringDOBWithInvalid()
	{
		// Arrange
		var user = CreateFakeUser();
		user.DOB = DateOnly.Parse("1990-05-05");
		var invalidDto = new UserEditProfileDTO { DOB = "invalid-date" };

		_userRepoMock.Setup(repo => repo.GetUserById(It.IsAny<Guid>(), default))
			.ReturnsAsync(user);
		_userRepoMock.Setup(repo => repo.EditProfile(It.IsAny<User>()))
			.ReturnsAsync(true);

		// Act
		var result = await _userServ.EditProfile(_userId, invalidDto);
		var jsonResult = Assert.IsType<JsonResult>(result);

		// Assert
		Assert.Equal(200, jsonResult.StatusCode);
		Assert.Equal(DateOnly.Parse("1990-05-05"), user.DOB);
		_userRepoMock.Verify(repo => repo.GetUserById(It.IsAny<Guid>(), default), Times.Once);
		_userRepoMock.Verify(repo => repo.AddImageVideo(It.IsAny<ImageVideo>()), Times.Never);
		_userRepoMock.Verify(repo => repo.UpdateImageVideo(It.IsAny<ImageVideo>()), Times.Never);
		_userRepoMock.Verify(repo => repo.EditProfile(It.IsAny<User>()), Times.Once);
	}

	[Fact]
	public async Task EditProfileAsync_Should_ThrowException_WhenUserNotFound()
	{
		// Arrange
		_userRepoMock.Setup(repo => repo.GetUserById(It.IsAny<Guid>(), default))
			.ReturnsAsync((User)null);

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.EditProfile(_userId, _userDto));
		_userRepoMock.Verify(repo => repo.GetUserById(It.IsAny<Guid>(), default), Times.Once);
		_userRepoMock.Verify(repo => repo.AddImageVideo(It.IsAny<ImageVideo>()), Times.Never);
		_userRepoMock.Verify(repo => repo.UpdateImageVideo(It.IsAny<ImageVideo>()), Times.Never);
		_userRepoMock.Verify(repo => repo.EditProfile(It.IsAny<User>()), Times.Never);
	}

	[Fact]
	public async Task EditProfileAsync_Should_ThrowException_WhenRepoThrows()
	{
		// Arrange
		_userRepoMock.Setup(repo => repo.GetUserById(It.IsAny<Guid>(), default))
			.ThrowsAsync(new Exception());

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _userServ.EditProfile(_userId, _userDto));
		_userRepoMock.Verify(repo => repo.GetUserById(It.IsAny<Guid>(), default), Times.Once);
		_userRepoMock.Verify(repo => repo.AddImageVideo(It.IsAny<ImageVideo>()), Times.Never);
		_userRepoMock.Verify(repo => repo.UpdateImageVideo(It.IsAny<ImageVideo>()), Times.Never);
		_userRepoMock.Verify(repo => repo.EditProfile(It.IsAny<User>()), Times.Never);
	}
}
