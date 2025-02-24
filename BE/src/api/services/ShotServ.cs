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
		Task<IActionResult> GetShots(ShotSearchFilterDTO filter);
	}
	public class ShotServ : IShotServ
	{
		private readonly IShotRepo _shotRepo;
		private readonly ICacheService _cacheService;
		public ShotServ(IShotRepo shotRepo, ICacheService cacheService)
		{
			_shotRepo = shotRepo;
			_cacheService = cacheService;
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

				await _cacheService.ClearWithPattern("shots");

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
		
		private string GenerateCacheKey(ShotSearchFilterDTO filter)
		{
			var keyParts = new List<string>();

			if (!string.IsNullOrEmpty(filter.UserName)) keyParts.Add($"username:{filter.UserName}"); 
			if (!string.IsNullOrEmpty(filter.UserEmail)) keyParts.Add($"useremail:{filter.UserEmail}");
			if (!string.IsNullOrEmpty(filter.UserCity)) keyParts.Add($"usercity:{filter.UserCity}");
			if (!string.IsNullOrEmpty(filter.UserEducation)) keyParts.Add($"usereducation:{filter.UserEducation}");
			if (!string.IsNullOrEmpty(filter.SpecialtyName)) keyParts.Add($"specialtyname:{filter.SpecialtyName}");
			if (!string.IsNullOrEmpty(filter.HtmlKeyword)) keyParts.Add($"htmlkeyword:{filter.HtmlKeyword}");
			if (!string.IsNullOrEmpty(filter.CssKeyword)) keyParts.Add($"csskeyword:{filter.CssKeyword}");
			if (filter.MinViews.HasValue) keyParts.Add($"minviews:{filter.MinViews}");
			if (filter.MaxViews.HasValue) keyParts.Add($"maxviews:{filter.MaxViews}");

			return $"shots{string.Join("|", keyParts)}";
		}

		public async Task<IActionResult> GetShots(ShotSearchFilterDTO filter)
		{
			try
			{
				var cacheKey = GenerateCacheKey(filter);
				
				var cacheShots = await _cacheService.Get<List<Shot>>(cacheKey);
				if (cacheShots != null)
					return SuccessResp.Ok(cacheShots);

				var shots = await _shotRepo.GetShots(filter);
				if(shots.Count == 0)
				{
					return ErrorResp.NotFound("No shot found");
				}

				await _cacheService.Set(cacheKey, shots, TimeSpan.FromMinutes(10));

				return SuccessResp.Ok(shots);
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}
	}
}
