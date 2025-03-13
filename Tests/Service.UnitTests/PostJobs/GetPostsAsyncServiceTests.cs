using BE.src.api.domains.DTOs.Post;
using BE.src.api.domains.Enum;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.PostJobs;
public class GetPostsAsyncServiceTests
{
	private readonly PostServ _postServ;
	private readonly Mock<IPostRepo> _postRepoMock;
	private readonly Mock<ICacheService> _cachServMock;
	private readonly Mock<INotificationRepo> _notificationRepoMock;
	private readonly Mock<ISpecialtyRepo> _specialtyRepoMock;
	private readonly Guid _userId = Guid.NewGuid();
	private readonly Guid _postId = Guid.NewGuid();
	public GetPostsAsyncServiceTests()
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

	private string GenerateCacheKey(PostJobFilterDTO filter)
	{
		var keyParts = new List<string>();

		if (!string.IsNullOrEmpty(filter.Title)) keyParts.Add($"Title:{filter.Title}");
		if (filter.WorkType.HasValue) keyParts.Add($"WorkType:{filter.WorkType}");
		if (!string.IsNullOrEmpty(filter.WorkLocation)) keyParts.Add($"WorkLocation:{filter.WorkLocation}");
		if (!string.IsNullOrEmpty(filter.CompanyName)) keyParts.Add($"Company:{filter.CompanyName}");
		if (filter.EmploymentType.HasValue) keyParts.Add($"EmploymentType:{filter.EmploymentType}");
		if (filter.MinExperience.HasValue) keyParts.Add($"MinExp:{filter.MinExperience}");
		if (filter.MaxExperience.HasValue) keyParts.Add($"MaxExp:{filter.MaxExperience}");
		if (!string.IsNullOrEmpty(filter.UserName)) keyParts.Add($"UserName:{filter.UserName}");
		if (!string.IsNullOrEmpty(filter.UserEmail)) keyParts.Add($"UserEmail:{filter.UserEmail}");
		if (!string.IsNullOrEmpty(filter.UserCity)) keyParts.Add($"UserCity:{filter.UserCity}");
		if (!string.IsNullOrEmpty(filter.UserEducation)) keyParts.Add($"Education:{filter.UserEducation}");
		if (!string.IsNullOrEmpty(filter.SpecialtyName)) keyParts.Add($"Specialty:{filter.SpecialtyName}");

		return $"posts{string.Join("|", keyParts)}";
	}

	[Fact]
	public async Task GetPosts_Should_ReturnCachedPosts_WhenCacheExists()
	{
		// Arrange
		var filter = new PostJobFilterDTO { Title = "Developer" };
		var cacheKey = GenerateCacheKey(filter);
		var cachedPosts = new List<PostJob>
		{
			new PostJob {
				Id = _postId,
				Title = "Developer",
				CompanyLogo = new ImageVideo { Url = "https://logo.com/image.png", Type = MediaTypeEnum.Image, IsMain = true},
				CompanyName = "TechCorp",
				WorkLocation = "Remote",
				WorkType = WorkTypeEnum.Remote,
				EmploymentType = EmploymentTypeEnum.FullTime,
				Experience = 1,
				Description = "Job description here",
				CompanyLink = "https://company.com",
				SpecialtyId = Guid.NewGuid(),
				Payment = 3000,
				Attachments = new List<Attachment>
				{
					new Attachment { Id = Guid.NewGuid(), FileName = "Resume.pdf", FileType = FileTypeEnum.PDF }
				}
			}
		};

		_cachServMock.Setup(cache => cache.Get<List<PostJob>>(cacheKey))
			.ReturnsAsync(cachedPosts);

		// Act
		var result = await _postServ.GetPosts(filter);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.Equal(cachedPosts, jsonResult.Value);
		_cachServMock.Verify(cache => cache.Get<List<PostJob>>(cacheKey), Times.Once);
		_postRepoMock.Verify(repo => repo.GetPosts(It.IsAny<PostJobFilterDTO>()), Times.Never);
		_cachServMock.Verify(cache => cache.Set(cacheKey, cachedPosts, TimeSpan.FromMinutes(10)), Times.Never);
	}

	[Fact]
	public async Task GetPostsAsync_Should_FetchFromRepository_WhenCacheIsEmpty()
	{
		// Arrange
		var filter = new PostJobFilterDTO { Title = "Developer" };
		var cacheKey = GenerateCacheKey(filter);
		var repoPosts = new List<PostJob>
		{
			new PostJob {
				Id = _postId,
				Title = "Developer",
				CompanyLogo = new ImageVideo { Url = "https://logo.com/image.png", Type = MediaTypeEnum.Image, IsMain = true},
				CompanyName = "TechCorp",
				WorkLocation = "Remote",
				WorkType = WorkTypeEnum.Remote,
				EmploymentType = EmploymentTypeEnum.FullTime,
				Experience = 1,
				Description = "Job description here",
				CompanyLink = "https://company.com",
				SpecialtyId = Guid.NewGuid(),
				Payment = 3000,
				Attachments = new List<Attachment>
				{
					new Attachment { Id = Guid.NewGuid(), FileName = "Resume.pdf", FileType = FileTypeEnum.PDF }
				}
			}
		};

		_cachServMock.Setup(cache => cache.Get<List<PostJob>>(cacheKey))
						 .ReturnsAsync((List<PostJob>)null);
		_postRepoMock.Setup(repo => repo.GetPosts(filter))
					 .ReturnsAsync(repoPosts);

		// Act
		var result = await _postServ.GetPosts(filter);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		Assert.Equal(repoPosts, jsonResult.Value);
		_cachServMock.Verify(cache => cache.Get<List<PostJob>>(cacheKey), Times.Once);
		_postRepoMock.Verify(repo => repo.GetPosts(filter), Times.Once);
		_cachServMock.Verify(cache => cache.Set(cacheKey, repoPosts, TimeSpan.FromMinutes(10)), Times.Once);
	}

	[Fact]
	public async Task GetPostsAsync_Should_ThrowException_WhenNoPostsFound()
	{
		// Arrange
		var filter = new PostJobFilterDTO { Title = "NonExistingJob" };
		var cacheKey = GenerateCacheKey(filter);
		var repoPosts = new List<PostJob>
		{
			new PostJob {
				Id = _postId,
				Title = "Developer",
				CompanyLogo = new ImageVideo { Url = "https://logo.com/image.png", Type = MediaTypeEnum.Image, IsMain = true},
				CompanyName = "TechCorp",
				WorkLocation = "Remote",
				WorkType = WorkTypeEnum.Remote,
				EmploymentType = EmploymentTypeEnum.FullTime,
				Experience = 1,
				Description = "Job description here",
				CompanyLink = "https://company.com",
				SpecialtyId = Guid.NewGuid(),
				Payment = 3000,
				Attachments = new List<Attachment>
				{
					new Attachment { Id = Guid.NewGuid(), FileName = "Resume.pdf", FileType = FileTypeEnum.PDF }
				}
			}
		};

		_cachServMock.Setup(cache => cache.Get<List<PostJob>>(cacheKey))
						 .ReturnsAsync((List<PostJob>)null);
		_postRepoMock.Setup(repo => repo.GetPosts(filter))
					 .ReturnsAsync(new List<PostJob>());

		// Act & Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _postServ.GetPosts(filter));
		_cachServMock.Verify(cache => cache.Get<List<PostJob>>(cacheKey), Times.Once);
		_postRepoMock.Verify(repo => repo.GetPosts(filter), Times.Once);
		_cachServMock.Verify(cache => cache.Set(cacheKey, repoPosts, TimeSpan.FromMinutes(10)), Times.Never);
	}

	[Fact]
	public void GenerateCacheKey_Should_GenerateCorrectKey()
	{
		// Arrange
		var filter = new PostJobFilterDTO
		{
			Title = "Dev",
			WorkType = WorkTypeEnum.Remote,
			WorkLocation = "NY",
			CompanyName = "TechCorp"
		};

		var expectedKey = "postsTitle:Dev|WorkType:Remote|WorkLocation:NY|Company:TechCorp";

		// Act
		var generatedKey = GenerateCacheKey(filter);

		// Assert
		Assert.Equal(expectedKey, generatedKey);
	}

	[Fact]
	public async Task GetPostsAsync_Should_HandleException_Gracefully()
	{
		// Arrange
		var filter = new PostJobFilterDTO { Title = "ErrorJob" };
		var cacheKey = GenerateCacheKey(filter);
		var repoPosts = new List<PostJob>
		{
			new PostJob {
				Id = _postId,
				Title = "Developer",
				CompanyLogo = new ImageVideo { Url = "https://logo.com/image.png", Type = MediaTypeEnum.Image, IsMain = true},
				CompanyName = "TechCorp",
				WorkLocation = "Remote",
				WorkType = WorkTypeEnum.Remote,
				EmploymentType = EmploymentTypeEnum.FullTime,
				Experience = 1,
				Description = "Job description here",
				CompanyLink = "https://company.com",
				SpecialtyId = Guid.NewGuid(),
				Payment = 3000,
				Attachments = new List<Attachment>
				{
					new Attachment { Id = Guid.NewGuid(), FileName = "Resume.pdf", FileType = FileTypeEnum.PDF }
				}
			}
		};

		_cachServMock.Setup(cache => cache.Get<List<PostJob>>(cacheKey))
						 .ThrowsAsync(new Exception("Cache service failure"));

		// Act & Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _postServ.GetPosts(filter));
		_cachServMock.Verify(cache => cache.Get<List<PostJob>>(cacheKey), Times.Once);
		_postRepoMock.Verify(repo => repo.GetPosts(filter), Times.Never);
		_cachServMock.Verify(cache => cache.Set(cacheKey, repoPosts, TimeSpan.FromMinutes(10)), Times.Never);
	}
}
