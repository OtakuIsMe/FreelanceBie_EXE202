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
		Task<IActionResult> ApplyJob(Guid userId, Guid postId);
		Task<IActionResult> PostJobDetail(Guid? userId, string postCode);
	}
	public class PostServ : IPostServ
	{
		private readonly IPostRepo _postRepo;
		private readonly INotificationRepo _notificationRepo;
		public PostServ(IPostRepo postRepo, INotificationRepo notificationRepo)
		{
			_postRepo = postRepo;
			_notificationRepo = notificationRepo;
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
					PostId = postId
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

		public async Task<IActionResult> PostJobDetail(Guid? userId, string postCode)
		{
			try
			{
				return SuccessResp.Ok("Hey");
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}
	}
}
