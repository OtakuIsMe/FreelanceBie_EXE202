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
		Task<IActionResult> PostJobDetail(Guid? userId, string postCode);
		Task<IActionResult> HistoryHiring(Guid postId);
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
						FileContent = memoryStream.ToArray(),
						FileType = FileTypeEnum.PDF
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

		public async Task<IActionResult> PostJobDetail(Guid? userId, string postCode)
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
	}
}
