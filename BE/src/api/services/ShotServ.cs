using BE.src.api.domains.DTOs.Shot;
using BE.src.api.domains.Enum;
using BE.src.api.domains.Model;
using BE.src.api.helpers;
using BE.src.api.repositories;
using BE.src.api.shared.Type;
using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.services
{
	public interface IShotServ
	{
		Task<IActionResult> AddShotData(ShotAddData data);
		Task<IActionResult> LikeShot(Guid userId, Guid shotId, bool state);
	}
	public class ShotServ : IShotServ
	{
		private readonly IShotRepo _shotRepo;
		public ShotServ(IShotRepo shotRepo)
		{
			_shotRepo = shotRepo;
		}
		public async Task<IActionResult> AddShotData(ShotAddData data)
		{
			try
			{
				Shot newShot = new()
				{
					UserId = data.UserId,
					SpecialtyId = data.SpecialtyId,
					Html = data.Html,
					Css = data.Css,
					View = 0
				};
				List<ImageVideo> imageVideos = [];
				foreach (var (file, index) in data.Images.Select((file, index) => (file, index)))
				{
					var fileUrl = await Utils.GenerateAzureUrl(MediaTypeEnum.Image,
										file, $"shot/{index}/{Utils.HashObject(newShot.Id)}");
					ImageVideo newImageVideo = new()
					{
						Type = MediaTypeEnum.Image,
						Url = fileUrl
					};
					imageVideos.Add(newImageVideo);
				}
				newShot.ImageVideos = imageVideos;
				bool isCreated = await _shotRepo.CreateShot(newShot);
				if (!isCreated)
				{
					return ErrorResp.BadRequest("Cant create shot");
				}
				return SuccessResp.Created("Add Shot Success");
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}

		public async Task<IActionResult> LikeShot(Guid userId, Guid shotId, bool state)
		{
			try
			{
				var like = await _shotRepo.GetLike(userId, shotId);
				if (state)
				{
					if (like == null)
					{
						Like newLike = new()
						{
							UserId = userId,
							ShotId = shotId
						};
						await _shotRepo.CreateLikeShot(newLike);
					}
					return SuccessResp.Ok("Like shot successful");
				}
				else
				{
					if (like != null)
					{
						await _shotRepo.DeleteLikeShot(like);
					}
					return SuccessResp.Ok("UnLike shot successful");
				}
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}
	}
}
