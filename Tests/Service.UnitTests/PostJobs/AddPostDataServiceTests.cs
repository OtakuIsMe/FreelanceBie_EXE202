using BE.src.api.domains.DTOs.Post;
using BE.src.api.domains.Enum;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using DotNetEnv;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.PostJobs;
public class AddPostDataServiceTests
{
	private readonly PostServ _postServ;
	private readonly Mock<IPostRepo> _postRepoMock;
	private readonly Mock<ICacheService> _cachServMock;
	private readonly Mock<INotificationRepo> _notificationRepoMock;
	private readonly Mock<ISpecialtyRepo> _specialtyRepoMock;
	private readonly Guid _userId = Guid.NewGuid();
	public AddPostDataServiceTests()
	{
		string envPath = Path.GetFullPath(Path.Combine
			(AppDomain.CurrentDomain.BaseDirectory, "../../../../../BE/.env"));

		Env.Load(envPath);

		_postRepoMock = new Mock<IPostRepo>();
		_notificationRepoMock = new Mock<INotificationRepo>();
		_cachServMock = new Mock<ICacheService>();
		_specialtyRepoMock = new Mock<ISpecialtyRepo>();

		_postServ = new PostServ(
			_postRepoMock.Object,
			_cachServMock.Object,
			_notificationRepoMock.Object,
			_specialtyRepoMock.Object);
	}

	[Fact]
	public async Task AddPostDataAsync_Should_ReturnSuccess_WhenPostIsCreated()
	{
		// Arrange
		var postData = new PostAddData
		{
			Title = "Test Post",
			Description = "This is a test description",
			Specialty = "Software",
			WorkType = WorkTypeEnum.Remote,
			WorkLocation = "USA",
			CompanyName = "TestCorp",
			EmploymentType = EmploymentTypeEnum.FullTime,
			Experience = 3,
			CompanyLink = "https://company.com",
			Payment = 5000,
			CompanyLogo = new FormFile(Stream.Null, 0, 0, "Data", "logo.png"),
			Files = new List<IFormFile>()
		};

		var specialty = new Specialty { Id = Guid.NewGuid(), Name = "Software" };

		_specialtyRepoMock.Setup(repo => repo.GetSpecialtyByName("Software"))
			.ReturnsAsync(specialty);
		_postRepoMock.Setup(repo => repo.CreatePost(It.IsAny<PostJob>()))
			.ReturnsAsync(true);
		_cachServMock.Setup(cache => cache.ClearWithPattern("posts"))
			.Returns(Task.CompletedTask);

		// Act
		var result = await _postServ.AddPostData(_userId, postData);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.Equal(201, jsonResult.StatusCode);
		_specialtyRepoMock.Verify(repo => repo.GetSpecialtyByName("Software"), Times.Once);
		_postRepoMock.Verify(repo => repo.CreatePost(It.IsAny<PostJob>()), Times.Once);
		_cachServMock.Verify(cache => cache.ClearWithPattern("posts"), Times.Once);
	}

	[Fact]
	public async Task AddPostDataAsync_Should_ReturnSuccess_WhenSpecialtyNotFound()
	{
		// Arrange
		var postData = new PostAddData
		{
			Title = "Test Post",
			Description = "This is a test description",
			Specialty = "Unknown",
			WorkType = WorkTypeEnum.Remote,
			WorkLocation = "USA",
			CompanyName = "TestCorp",
			EmploymentType = EmploymentTypeEnum.FullTime,
			Experience = 3,
			CompanyLink = "https://company.com",
			Payment = 5000,
			CompanyLogo = new FormFile(Stream.Null, 0, 0, "Data", "logo.png"),
			Files = new List<IFormFile>()
		};

		_specialtyRepoMock.Setup(repo => repo.GetSpecialtyByName("Unknown"))
			.ReturnsAsync((Specialty)null); // find specialty null
		_specialtyRepoMock.Setup(repo => repo.AddSpecialty(It.IsAny<Specialty>()))
			.ReturnsAsync(true);
		_postRepoMock.Setup(repo => repo.CreatePost(It.IsAny<PostJob>()))
			.ReturnsAsync(true);

		// Act
		var result = await _postServ.AddPostData(_userId, postData);

		// Assert
		Assert.NotNull(result);
		_specialtyRepoMock.Verify(repo => repo.GetSpecialtyByName("Unknown"), Times.Once);
		_specialtyRepoMock.Verify(repo => repo.AddSpecialty(It.IsAny<Specialty>()), Times.Once);
		_postRepoMock.Verify(repo => repo.CreatePost(It.IsAny<PostJob>()), Times.Once);
	}

	[Fact]
	public async Task AddPostDataAsync_Should_ThrowException_WhenPostCreationFails()
	{
		// Arrange
		var postData = new PostAddData
		{
			Title = "Test Post",
			Description = "This is a test description",
			Specialty = "Software",
			WorkType = WorkTypeEnum.Remote,
			WorkLocation = "USA",
			CompanyName = "TestCorp",
			EmploymentType = EmploymentTypeEnum.FullTime,
			Experience = 3,
			CompanyLink = "https://company.com",
			Payment = 5000,
			CompanyLogo = new FormFile(Stream.Null, 0, 0, "Data", "logo.png"),
			Files = new List<IFormFile>()
		};

		var specialty = new Specialty { Id = Guid.NewGuid(), Name = "Software" };

		_specialtyRepoMock.Setup(repo => repo.GetSpecialtyByName("Software"))
			.ReturnsAsync(specialty);
		_postRepoMock.Setup(repo => repo.CreatePost(It.IsAny<PostJob>()))
			.ReturnsAsync(false); // Post creation fails

		// Act & Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _postServ.AddPostData(_userId, postData));
		_specialtyRepoMock.Verify(repo => repo.GetSpecialtyByName("Software"), Times.Once);
		_postRepoMock.Verify(repo => repo.CreatePost(It.IsAny<PostJob>()), Times.Once);
	}

	[Fact]
	public async Task AddPostDataAsync_Should_ThrowException_WhenCacheClearingFails()
	{
		// Arrange
		var postData = new PostAddData
		{
			Title = "Test Post",
			Description = "This is a test description",
			Specialty = "Software",
			WorkType = WorkTypeEnum.Remote,
			WorkLocation = "USA",
			CompanyName = "TestCorp",
			EmploymentType = EmploymentTypeEnum.FullTime,
			Experience = 3,
			CompanyLink = "https://company.com",
			Payment = 5000,
			CompanyLogo = new FormFile(Stream.Null, 0, 0, "Data", "logo.png"),
			Files = new List<IFormFile>()
		};

		var specialty = new Specialty { Id = Guid.NewGuid(), Name = "Software" };

		_specialtyRepoMock.Setup(repo => repo.GetSpecialtyByName("Software"))
			.ReturnsAsync(specialty);
		_postRepoMock.Setup(repo => repo.CreatePost(It.IsAny<PostJob>()))
			.ReturnsAsync(true);
		_cachServMock.Setup(cache => cache.ClearWithPattern("posts"))
			.Throws(new Exception("Cache Error")); // remove cache key fail

		// Act & Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _postServ.AddPostData(_userId, postData));
		_specialtyRepoMock.Verify(repo => repo.GetSpecialtyByName("Software"), Times.Once);
		_postRepoMock.Verify(repo => repo.CreatePost(It.IsAny<PostJob>()), Times.Once);
		_cachServMock.Verify(cache => cache.ClearWithPattern("posts"), Times.Once);
	}

	[Fact]
	public async Task AddPostDataAsync_Should_ThrowException_WhenCreateSpecialtyFails()
	{
		// Arrange
		var postData = new PostAddData
		{
			Title = "Test Post",
			Description = "This is a test description",
			Specialty = "Unknown",
			WorkType = WorkTypeEnum.Remote,
			WorkLocation = "USA",
			CompanyName = "TestCorp",
			EmploymentType = EmploymentTypeEnum.FullTime,
			Experience = 3,
			CompanyLink = "https://company.com",
			Payment = 5000,
			CompanyLogo = new FormFile(Stream.Null, 0, 0, "Data", "logo.png"),
			Files = new List<IFormFile>()
		};

		_specialtyRepoMock.Setup(repo => repo.GetSpecialtyByName("Unknown"))
			.ReturnsAsync((Specialty)null);
		_specialtyRepoMock.Setup(repo => repo.AddSpecialty(It.IsAny<Specialty>()))
			.ThrowsAsync(new Exception("Database Errors"));

		// Act
		
		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _postServ.AddPostData(_userId, postData));
		_specialtyRepoMock.Verify(repo => repo.GetSpecialtyByName("Unknown"), Times.Once);
		_specialtyRepoMock.Verify(repo => repo.AddSpecialty(It.IsAny<Specialty>()), Times.Once);
		_postRepoMock.Verify(repo => repo.CreatePost(It.IsAny<PostJob>()), Times.Never);
	}

	[Fact]
	public async Task AddPostData_Should_ProcessAttachmentsCorrectly()
	{
		// Arrange
		var fileMock1 = new Mock<IFormFile>();
		var fileMock2 = new Mock<IFormFile>();

		var content1 = "Fake PDF File Content";
		var content2 = "Fake DOCX File Content";
		var fileName1 = "test1.pdf";
		var fileName2 = "test2.docx";

		var memoryStream1 = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content1));
		var memoryStream2 = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content2));

		fileMock1.Setup(f => f.OpenReadStream()).Returns(memoryStream1);
		fileMock1.Setup(f => f.FileName).Returns(fileName1);
		fileMock1.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
				.Returns<Stream, System.Threading.CancellationToken>((stream, token) => memoryStream1.CopyToAsync(stream));

		fileMock2.Setup(f => f.OpenReadStream()).Returns(memoryStream2);
		fileMock2.Setup(f => f.FileName).Returns(fileName2);
		fileMock2.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
				.Returns<Stream, System.Threading.CancellationToken>((stream, token) => memoryStream2.CopyToAsync(stream));

		var mockFiles = new List<IFormFile> { fileMock1.Object, fileMock2.Object };

		// Act
		List<Attachment> attachments = new();
		foreach (IFormFile fileAttachment in mockFiles)
		{
			using var memoryStream = new MemoryStream();
			await fileAttachment.CopyToAsync(memoryStream);

			string fileExtension = Path.GetExtension(fileAttachment.FileName).ToLower();
			FileTypeEnum fileType = fileExtension switch
			{
				".pdf" => FileTypeEnum.PDF,
				".docx" => FileTypeEnum.DOCX,
				_ => FileTypeEnum.Unknown
			};

			Attachment attachment = new Attachment
			{
				PostId = Guid.NewGuid(),
				FileName = fileAttachment.FileName,
				FileContent = memoryStream.ToArray(),
				FileType = fileType
			};

			attachments.Add(attachment);
		}

		// Assert
		Assert.Equal(2, attachments.Count);
		Assert.Contains(attachments, a => a.FileName == "test1.pdf" && a.FileType == FileTypeEnum.PDF);
		Assert.Contains(attachments, a => a.FileName == "test2.docx" && a.FileType == FileTypeEnum.DOCX);
		Assert.All(attachments, a => Assert.NotEmpty(a.FileContent));
	}
}
