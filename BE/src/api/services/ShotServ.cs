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
		Task<IActionResult> AddShotData(ShotAddData data, Guid UserId);
		Task<IActionResult> LikeShot(Guid userId, Guid shotId, bool state);
		Task<IActionResult> ShotOwner(Guid userId);
		Task<IActionResult> GetShotDetail(Guid? userId, Guid shotCode);
		Task<IActionResult> GetShots(ShotSearchFilterDTO filter);
		Task<IActionResult> OtherShots(Guid shotId);
		Task<IActionResult> ShotRandom(int item);
		Task<IActionResult> ListShotLiked(Guid userId);
		Task<IActionResult> ListShotView(int page, int count);
	}
	public class ShotServ : IShotServ
	{
		private readonly IShotRepo _shotRepo;
		private readonly ICacheService _cacheService;
		private readonly ISpecialtyRepo _specialtyRepo;
		public ShotServ(IShotRepo shotRepo, ICacheService cacheService, ISpecialtyRepo specialtyRepo)
		{
			_shotRepo = shotRepo;
			_specialtyRepo = specialtyRepo;
			_cacheService = cacheService;
		}
		public async Task<IActionResult> AddShotData(ShotAddData data, Guid UserId)
		{
			try
			{
				List<Specialty> specialties = new();
				foreach (string specialtyName in data.Specialties)
				{
					var specialty = await _specialtyRepo.GetSpecialtyByName(specialtyName);
					if (specialty == null)
					{
						var newSpecialty = new Specialty
						{
							Name = specialtyName
						};
						await _specialtyRepo.AddSpecialty(newSpecialty);
						specialties.Add(newSpecialty);
					}
					else
					{
						specialties.Add(specialty);
					}
				}
				Shot newShot = new()
				{
					Title = data.Title,
					UserId = UserId,
					Specialties = specialties,
					Html = data.Html,
					View = 0
				};
				List<ImageVideo> imageVideos = [];
				foreach (var (img, index) in data.Images.Select((file, index) => (file, index)))
				{
					var newImageUrl = await Utils.GenerateAzureUrl(MediaTypeEnum.Image,
										img.File, $"shot/{Utils.HashObject(newShot.Id)}/{index}");
					ImageVideo newImageVideo = new()
					{
						Type = MediaTypeEnum.Image,
						Url = newImageUrl,
						IsMain = index == 0 ? true : false
					};
					newShot.Html = newShot.Html.Replace(img.Replace, newImageUrl);
					imageVideos.Add(newImageVideo);
				}
				newShot.ImageVideos = imageVideos;
				bool isCreated = await _shotRepo.CreateShot(newShot);
				if (!isCreated)
				{
					throw new ApplicationException("Cant create shot");
				}

				await _cacheService.ClearWithPattern("shots");

				return SuccessResp.Created("Add Shot Success");
			}
			catch (System.Exception ex)
			{
				throw new ApplicationException(ex.Message);
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
				throw new ApplicationException(ex.Message);
			}
		}

		private string GenerateCacheKey(ShotSearchFilterDTO filter)
		{
			var keyParts = new List<string>();

			if (!string.IsNullOrEmpty(filter.UserName)) keyParts.Add($"username:{filter.UserName}");
			if (!string.IsNullOrEmpty(filter.UserEmail)) keyParts.Add($"useremail:{filter.UserEmail}");
			if (!string.IsNullOrEmpty(filter.UserEducation)) keyParts.Add($"usereducation:{filter.UserEducation}");
			if (!string.IsNullOrEmpty(filter.SpecialtyName)) keyParts.Add($"specialtyname:{filter.SpecialtyName}");
			if (!string.IsNullOrEmpty(filter.HtmlKeyword)) keyParts.Add($"htmlkeyword:{filter.HtmlKeyword}");
			if (!string.IsNullOrEmpty(filter.CssKeyword)) keyParts.Add($"csskeyword:{filter.CssKeyword}");
			if (filter.MinViews.HasValue) keyParts.Add($"minviews:{filter.MinViews}");
			if (filter.MaxViews.HasValue) keyParts.Add($"maxviews:{filter.MaxViews}");

			return $"shots{string.Join("|", keyParts)}";
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
				throw new ApplicationException(ex.Message);
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
					Image = s.ImageVideos.FirstOrDefault(i => i.IsMain == true).Url,
					CountView = countView,
					CountLike = countLike,
					Title = s.Title,
					Id = s.Id,
					Specialties = s.Specialties.Select(s => s.Name).ToList(),
					DatePosted = s.CreateAt,
					User = new UserShotCard()
					{
						Username = s.User.Username,
						Image = s.User.ImageVideos.FirstOrDefault()?.Url ?? "https://i.kym-cdn.com/photos/images/newsfeed/002/601/167/c81"
					}
				};
				shotCards.Add(newShotCard);
			}
			return shotCards;
		}

		public async Task<IActionResult> GetShotDetail(Guid? userId, Guid shotCode)
		{
			try
			{
				var shot = await _shotRepo.GetShotByShotCode(shotCode);
				if (shot == null)
				{
					throw new ApplicationException("Cant find shot");
				}
				if (shot.User.ImageVideos == null || shot.User.Slogan == null)
				{
					throw new ApplicationException("Owner is not set up profile");
				}
				var shotDetail = new ShotDetail
				{
					Title = shot.Title,
					Html = shot.Html,
					Owner = new ShotOwner
					{
						Id = shot.UserId,
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
				throw new ApplicationException(ex.Message);
			}
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
				if (shots.Count == 0)
				{
					throw new ApplicationException("No shot found");
				}

				await _cacheService.Set(cacheKey, shots, TimeSpan.FromMinutes(10));

				return SuccessResp.Ok(shots);
			}
			catch (System.Exception ex)
			{
				throw new ApplicationException(ex.Message);
			}
		}

		public async Task<IActionResult> OtherShots(Guid shotId)
		{
			try
			{
				Shot? shot = await _shotRepo.GetShotById(shotId);
				if (shot == null)
				{
					return ErrorResp.BadRequest("Cant found shot");
				}
				List<Shot> shotOwner = await _shotRepo.GetShotsByUser(shot.UserId);
				shotOwner = shotOwner.Where(s => s.Id != shot.Id).ToList();
				var otherShots = shotOwner.Take(4).ToList();
				return SuccessResp.Ok(otherShots);
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}

		public async Task<IActionResult> ShotRandom(int item)
		{
			try
			{
				var shots = await _shotRepo.GetShotRandom(item);
				return SuccessResp.Ok(shots);
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}

		public async Task<IActionResult> ListShotLiked(Guid userId)
		{
			try
			{
				return SuccessResp.Ok(await _shotRepo.GetShotLikeds(userId));
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}
		public async Task<IActionResult> ListShotView(int page, int count)
		{
			try
			{
				return SuccessResp.Ok(await _shotRepo.ListShotView(page, count));
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}
	}
}
