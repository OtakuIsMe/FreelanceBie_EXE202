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
		Task<IActionResult> ShotOwner(Guid userId);
		Task<IActionResult> GetShotDetail(Guid? userId, string shotCode);
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
					Title = data.Title,
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

		public async Task<IActionResult> ShotOwner(Guid userId)
		{
			try
			{
				var shots = await _shotRepo.GetShotsByUser(userId);
				var shotCards = await GetShotCards(shots);
				return SuccessResp.Ok(shotCards);
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}
		private async Task<List<ShotCard>> GetShotCards(List<Shot> shots)
		{
			List<ShotCard> shotCards = [];
			foreach (Shot s in shots)
			{
				int countLike = await _shotRepo.GetLikeCount(s.Id);
				int countView = await _shotRepo.GetViewCount(s.Id);
				ShotCard newShotCard = new()
				{
					Image = s.ImageVideos.First().Url,
					CountView = countView,
					CountLike = countLike,
					User = new UserShotCard()
					{
						Username = s.User.Username,
						Image = s.User.ImageVideos.First().Url
					}
				};
				shotCards.Add(newShotCard);
			}
			return shotCards;
		}

		public async Task<IActionResult> GetShotDetail(Guid? userId, string shotCode)
		{
			try
			{
				var shot = await _shotRepo.GetShotByShotCode(shotCode);
				if (shot == null)
				{
					return ErrorResp.BadRequest("Cant find shot");
				}
				if (shot.User.ImageVideos == null || shot.User.Slogan == null)
				{
					return ErrorResp.BadRequest("Owner is not set up profile");
				}
				var shotDetail = new ShotDetail
				{
					Title = shot.Title,
					Html = shot.Html,
					Css = shot.Css,
					Owner = new ShotOwner
					{
						Image = shot.User.ImageVideos.First().Url,
						Name = shot.User.Username,
						Status = "Available",
						Slogan = shot.User.Slogan
					}
				};
				if (userId.HasValue)
				{
					shotDetail.User = new ShotUser
					{
						IsLiked = await _shotRepo.IsLikedShot(userId.Value, shot.Id),
						IsSaved = await _shotRepo.IsSaved(userId.Value, shot.Id)
					};
				}
				return SuccessResp.Ok(shotDetail);
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}
	}
}
