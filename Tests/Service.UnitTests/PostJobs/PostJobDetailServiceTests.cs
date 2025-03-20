using BE.src.api.domains.DTOs.Post;
using BE.src.api.domains.Enum;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.services;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Service.UnitTests.PostJobs;
public class PostJobDetailServiceTests
{
	private readonly PostServ _postServ;
	private readonly Mock<IPostRepo> _postRepoMock;
	private readonly Mock<ICacheService> _cachServMock;
	private readonly Mock<INotificationRepo> _notificationRepoMock;
	private readonly Mock<ISpecialtyRepo> _specialtyRepoMock;
	private readonly Guid _userId = Guid.NewGuid();
	private readonly Guid _postId = Guid.NewGuid();
	public PostJobDetailServiceTests()
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
	public async Task PostJobDetailAsync_Should_ReturnPostDetail_WhenPostExists_WithoutUserId()
	{
		// Arrange
		var post = new PostJob
		{
			Id = _postId,
			Title = "Software Engineer",
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
		};

		_postRepoMock.Setup(repo => repo.GetPostJobByCode(_postId))
			.ReturnsAsync(post);

		// Act
		var result = await _postServ.PostJobDetail(null, _postId);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result); // json
		var postDetail = Assert.IsType<PostDetail>(jsonResult.Value); // convert json equal to object
		Assert.Equal("Software Engineer", postDetail.Title);
		Assert.Equal("https://logo.com/image.png", postDetail.CompanyLogo);
		Assert.Equal("TechCorp", postDetail.CompanyName);
		Assert.Single(postDetail.AttachmentPosts);
		_postRepoMock.Verify(repo => repo.GetPostJobByCode(_postId), Times.Once);
		_postRepoMock.Verify(repo => repo.IsApply(_userId, _postId), Times.Never);
		_postRepoMock.Verify(repo => repo.IsSaved(_userId, _postId), Times.Never);
	}

	[Fact]
	public async Task PostJobDetailAsync_Should_ReturnPostDetail_WhenPostExists_WithUserId()
	{
		// Arrange
		var post = new PostJob
		{
			Id = _postId,
			Title = "Software Engineer",
			CompanyLogo = new ImageVideo { Url = "https://logo.com/image.png", Type = MediaTypeEnum.Image, IsMain = true },
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
		};

		_postRepoMock.Setup(repo => repo.GetPostJobByCode(_postId)).ReturnsAsync(post);
		_postRepoMock.Setup(repo => repo.IsApply(_userId, _postId)).ReturnsAsync(true);
		_postRepoMock.Setup(repo => repo.IsSaved(_userId, _postId)).ReturnsAsync(true);

		// Act
		var result = await _postServ.PostJobDetail(_userId, _postId);

		// Assert
		var jsonResult = Assert.IsType<JsonResult>(result);
		var postDetail = Assert.IsType<PostDetail>(jsonResult.Value);
		Assert.Equal("Software Engineer", postDetail.Title);
		Assert.Equal("https://logo.com/image.png", postDetail.CompanyLogo);
		Assert.Equal("TechCorp", postDetail.CompanyName);
		Assert.Equal(WorkTypeEnum.Remote, postDetail.WorkType);
		Assert.True(postDetail.User.IsApplied);
		Assert.True(postDetail.User.IsSaved);
		_postRepoMock.Verify(repo => repo.GetPostJobByCode(_postId), Times.Once);
		_postRepoMock.Verify(repo => repo.IsApply(_userId, _postId), Times.Once);
		_postRepoMock.Verify(repo => repo.IsSaved(_userId, _postId), Times.Once);
	}

	[Fact]
	public async Task PostJobDetail_Should_ThrowException_WhenPostNotFound()
	{
		// Arrange
		_postRepoMock.Setup(repo => repo.GetPostJobByCode(_postId)).ReturnsAsync((PostJob)null);

		// Act

		// Assert
		await Assert.ThrowsAsync<ApplicationException>(() => _postServ.PostJobDetail(null, _postId));
		_postRepoMock.Verify(repo => repo.GetPostJobByCode(_postId), Times.Once);
		_postRepoMock.Verify(repo => repo.IsApply(_userId, _postId), Times.Never);
		_postRepoMock.Verify(repo => repo.IsSaved(_userId, _postId), Times.Never);
	}
}
