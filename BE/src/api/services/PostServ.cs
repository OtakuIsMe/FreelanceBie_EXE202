using BE.src.api.domains.DTOs.Post;
using BE.src.api.domains.Enum;
using BE.src.api.domains.Model;
using BE.src.api.helpers;
using BE.src.api.repositories;
using BE.src.api.shared.Type;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.services
{
	public interface IPostServ
	{
		Task<IActionResult> AddPostData(Guid userId, PostAddData data);
		Task<IActionResult> ApplyJob(Guid userId, Guid postId);
		Task<IActionResult> PostJobDetail(Guid? userId, Guid postCode);
		Task<IActionResult> HistoryHiring(Guid postId);
		Task<IActionResult> GetPosts(PostJobFilterDTO filter);
	}
	public class PostServ : IPostServ
	{
		private readonly IPostRepo _postRepo;
		private readonly ICacheService _cacheService;
		private readonly INotificationRepo _notificationRepo;
		private readonly ISpecialtyRepo _specialtyRepo;
		public PostServ(IPostRepo postRepo, ICacheService cacheService, INotificationRepo notificationRepo, ISpecialtyRepo specialtyRepo)
		{
			_postRepo = postRepo;
			_notificationRepo = notificationRepo;
			_cacheService = cacheService;
			_specialtyRepo = specialtyRepo;
		}

		public async Task<IActionResult> AddPostData(Guid userId, PostAddData data)
		{
			try
			{
				var specialty = await _specialtyRepo.GetSpecialtyByName(data.Specialty);

				if (specialty == null)
				{
					var newSpecialty = new Specialty
					{
						Name = data.Specialty
					};
					await _specialtyRepo.AddSpecialty(newSpecialty);
					specialty = newSpecialty;
				}

				PostJob newPost = new()
				{
					Title = data.Title,
					Description = data.Description,
					WorkType = data.WorkType,
					WorkLocation = data.WorkLocation,
					CompanyName = data.CompanyName,
					EmploymentType = data.EmploymentType,
					Experience = data.Experience,
					UserId = userId,
					SpecialtyId = specialty.Id,
					CompanyLink = data.CompanyLink,
					Payment = data.Payment
				};

				var fileUrl = await Utils.GenerateAzureUrl(MediaTypeEnum.Image,
										data.CompanyLogo, $"company/{Utils.HashObject(newPost.Id)}");
				ImageVideo newImageVideo = new()
				{
					Type = MediaTypeEnum.Image,
					Url = fileUrl,
					IsMain = false
				};
				newPost.CompanyLogo = newImageVideo;

				List<Attachment> attachments = [];
				foreach (IFormFile fileAttachment in data.Files)
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
						PostId = newPost.Id,
						FileName = fileAttachment.FileName,
						FileContent = memoryStream.ToArray(),
						FileType = fileType
					};

					attachments.Add(attachment);
				}
				newPost.Attachments = attachments;

				var isCreated = await _postRepo.CreatePost(newPost);
				if (!isCreated)
				{
					return ErrorResp.BadRequest("Cant create post");
				}

				await _cacheService.ClearWithPattern("posts");

				return SuccessResp.Created("Add Post Success");
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}

		public async Task<IActionResult> ApplyJob(Guid userId, Guid postId)
		{
			try
			{
				var userApply = new UserApply
				{
					UserId = userId,
					PostId = postId,
					Status = ApplyStatusEnum.Waiting
				};
				var notification = await _notificationRepo.GetNotificationByPostId(postId);
				if (notification == null)
				{
					PostJob post = await _postRepo.GetPostJobById(postId);
					var newNotification = new Notification
					{
						Message = "has applied to your job post",
						UserId = post.UserId,
						PostId = postId,
						CountUser = 1
					};
					await _notificationRepo.CreateNotification(newNotification);
				}
				else
				{
					notification.CountUser = notification.CountUser++;
					await _notificationRepo.UpdateNotification(notification);
				}
				await _postRepo.CreateUserApply(userApply);
				return SuccessResp.Created("Apply succes");
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}

		public async Task<IActionResult> HistoryHiring(Guid postId)
		{
			try
			{
				return SuccessResp.Ok("Ok");
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}

		public async Task<IActionResult> PostJobDetail(Guid? userId, Guid postCode)
		{
			try
			{
				var post = await _postRepo.GetPostJobByCode(postCode);
				if (post == null)
				{
					return ErrorResp.BadRequest("Cant find post");
				}
				var postDetail = new PostDetail
				{
					Title = post.Title,
					CompanyLogo = post.CompanyLogo.Url,
					CompanyName = post.CompanyName,
					WorkLocation = post.WorkLocation,
					WorkType = post.WorkType,
					EmploymentType = post.EmploymentType,
					Experience = post.Experience,
					Description = post.Description,
					AttachmentPosts = post.Attachments.Select(a =>
					new AttachmentPost
					{
						Id = a.Id,
						Name = a.FileName,
						Type = a.FileType.ToString()
					})
				};
				if (userId.HasValue)
				{
					postDetail.User = new UserPost
					{
						IsApplied = await _postRepo.IsApply(userId.Value, post.Id),
						IsSaved = await _postRepo.IsSaved(userId.Value, post.Id)
					};
				}
				return SuccessResp.Ok(postDetail);
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}

		public async Task<IActionResult> GetPosts(PostJobFilterDTO filter)
		{
			try
			{
				var cacheKey = GenerateCacheKey(filter);

				var cachedPosts = await _cacheService.Get<List<PostJob>>(cacheKey);
				if (cachedPosts != null)
					return SuccessResp.Ok(cachedPosts);

				var posts = await _postRepo.GetPosts(filter);
				if (posts.Count == 0)
				{
					return ErrorResp.NotFound("No post found");
				}

				await _cacheService.Set(cacheKey, posts, TimeSpan.FromMinutes(10));

				return SuccessResp.Ok(posts);
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
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
	}
}
