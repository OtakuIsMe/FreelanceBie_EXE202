using BE.src.api.domains.DTOs.Post;
using BE.src.api.domains.Enum;
using BE.src.api.domains.Model;
using BE.src.api.helpers;
using BE.src.api.repositories;
using BE.src.api.shared.Type;
using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.services
{
	public interface IPostServ
	{
		Task<IActionResult> AddPostData(Guid userId, PostAddData data);
		Task<IActionResult> ApplyJob(Guid userId, Guid PostId);
		Task<IActionResult> GetPosts(PostJobFilterDTO filter);
	}
	public class PostServ : IPostServ
	{
		private readonly IPostRepo _postRepo;
		private readonly ICacheService _cacheService;
		public PostServ(IPostRepo postRepo, ICacheService cacheService)
		{
			_postRepo = postRepo;
			_cacheService = cacheService;
		}

		public async Task<IActionResult> AddPostData(Guid userId, PostAddData data)
		{
			try
			{
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
					SpecialtyId = data.SpecialtyId,
				};

				var fileUrl = await Utils.GenerateAzureUrl(MediaTypeEnum.Image,
										data.CompanyLogo, $"company/{Utils.HashObject(newPost.Id)}");
				ImageVideo newImageVideo = new()
				{
					Type = MediaTypeEnum.Image,
					Url = fileUrl
				};
				newPost.CompanyLogo = newImageVideo;

				List<Attachment> attachments = [];
				foreach (IFormFile fileAttachment in newPost.Attachments)
				{
					using var memoryStream = new MemoryStream();
					await fileAttachment.CopyToAsync(memoryStream);

					Attachment attachment = new Attachment
					{
						PostId = newPost.Id,
						FileName = fileAttachment.FileName,
						FileContent = memoryStream.ToArray()
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

		public Task<IActionResult> ApplyJob(Guid userId, Guid PostId)
		{
			throw new NotImplementedException();
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
				if(posts.Count == 0)
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
