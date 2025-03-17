using BE.src.api.domains.Database;
using BE.src.api.domains.DTOs.Shot;
using BE.src.api.domains.Model;
using BE.src.api.helpers;
using Microsoft.EntityFrameworkCore;

namespace BE.src.api.repositories
{
	public interface IShotRepo
	{
		Task<bool> CreateShot(Shot shot);
		Task<Like?> GetLike(Guid userId, Guid shotId);
		Task<bool> CreateLikeShot(Like like);
		Task<bool> DeleteLikeShot(Like like);
		Task<List<Shot>> GetShotsByUser(Guid userId);
		Task<int> GetLikeCount(Guid ShotId);
		Task<int> GetViewCount(Guid ShotId);
		Task<Shot?> GetShotByShotCode(Guid shotCode);
		Task<bool> IsLikedShot(Guid UserId, Guid ShotId);
		Task<bool> IsSaved(Guid UserId, Guid ShotId);
		Task<List<Shot>> GetShots(ShotSearchFilterDTO filter);
		Task<Shot?> GetShotById(Guid id);
		Task<List<ShotView>> GetShotRandom(int index);
		Task<List<ShotLiked>> GetShotLikeds(Guid userId);
		Task<List<ShotView>> ListShotView(int page, int count);
	}
	public class ShotRepo : IShotRepo
	{
		private readonly FLBDbContext _context;
		public ShotRepo(FLBDbContext context)
		{
			_context = context;
		}
		public async Task<bool> CreateShot(Shot shot)
		{
			_context.Shots.Add(shot);
			return await _context.SaveChangesAsync() > 0;
		}
		public async Task<Like?> GetLike(Guid userId, Guid shotId)
		{
			return await _context.Likes.FirstOrDefaultAsync(l => l.UserId == userId && l.ShotId == shotId);
		}
		public async Task<bool> CreateLikeShot(Like like)
		{
			_context.Likes.Add(like);
			return await _context.SaveChangesAsync() > 0;
		}
		public async Task<bool> DeleteLikeShot(Like like)
		{
			_context.Likes.Remove(like);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<List<Shot>> GetShotsByUser(Guid userId)
		{
			return await _context.Shots.Where(s => s.UserId == userId)
										.Include(s => s.User)
											.ThenInclude(u => u.ImageVideos)
										.Include(s => s.ImageVideos.Where(i => i.IsMain == true))
										.Include(s => s.Specialties)
										.AsNoTracking()
										.ToListAsync();
		}
		public async Task<int> GetLikeCount(Guid ShotId)
		{
			return await _context.Likes.CountAsync(l => l.ShotId == ShotId);
		}
		public async Task<int> GetViewCount(Guid ShotId)
		{
			return await _context.ViewAnalysts.Where(va => va.ShotId == ShotId)
											.SumAsync(va => va.View);
		}

		public async Task<Shot?> GetShotByShotCode(Guid shotCode)
		{
			return await _context.Shots
						.Include(s => s.User)
							.ThenInclude(u => u.ImageVideos)
						.FirstOrDefaultAsync(s => s.Id == shotCode);
		}

		public async Task<bool> IsLikedShot(Guid UserId, Guid ShotId)
		{
			return await _context.Likes.AnyAsync(l => l.UserId == UserId
										&& l.ShotId == ShotId);
		}

		public async Task<bool> IsSaved(Guid UserId, Guid ShotId)
		{
			return await _context.Saves.AnyAsync(s => s.UserId == UserId
										&& s.ShotId == ShotId);
		}

		public async Task<List<Shot>> GetShots(ShotSearchFilterDTO filter)
		{
			var query = _context.Shots
							.Include(s => s.ImageVideos)
							.Include(s => s.Comments)
								.ThenInclude(c => c.User)
									.ThenInclude(u => u.ImageVideos)
							.Include(s => s.Likes)
								.ThenInclude(l => l.User)
									.ThenInclude(u => u.ImageVideos)
							.Include(s => s.User)
								.ThenInclude(u => u.ImageVideos)
							.Include(s => s.Specialties)
							.AsQueryable();

			if (!string.IsNullOrEmpty(filter.UserName))
				query = query.Where(s => s.User.Name.Contains(filter.UserName));

			if (!string.IsNullOrEmpty(filter.UserEmail))
				query = query.Where(s => s.User.Email.Contains(filter.UserEmail));

			if (!string.IsNullOrEmpty(filter.UserCity))
				query = query.Where(s => s.User.City.Contains(filter.UserCity));

			if (!string.IsNullOrEmpty(filter.UserEducation))
				query = query.Where(s => s.User.Education.Contains(filter.UserEducation));

			if (!string.IsNullOrEmpty(filter.SpecialtyName))
				query = query.Where(s => s.Specialties.Any(sp => sp.Name == filter.SpecialtyName));

			if (!string.IsNullOrEmpty(filter.HtmlKeyword))
				query = query.Where(s => s.Html.Contains(filter.HtmlKeyword));

			if (filter.MinViews.HasValue)
				query = query.Where(s => s.View >= filter.MinViews);

			if (filter.MaxViews.HasValue)
				query = query.Where(s => s.View <= filter.MaxViews);

			return await query.ToListAsync();
		}

		public async Task<Shot?> GetShotById(Guid id)
		{
			return await _context.Shots.FirstOrDefaultAsync(s => s.Id == id);
		}

		public async Task<List<ShotView>> GetShotRandom(int index)
		{
			var randomShotIds = await _context.Shots
				.OrderBy(x => EF.Functions.Random())
				.Take(index)
				.Select(s => s.Id)
				.ToListAsync();
			var shots = await _context.Shots
				.Where(s => randomShotIds.Contains(s.Id))
				.Include(s => s.User)
				.Include(s => s.ImageVideos.Where(i => i.IsMain))
				.Select(s => new
				{
					Shot = s,
					ImageUrl = s.ImageVideos.FirstOrDefault(i => i.IsMain).Url
				})
				.ToListAsync();
			var result = new List<ShotView>();

			foreach (var item in shots)
			{
				result.Add(new ShotView
				{
					Id = item.Shot.Id,
					Title = item.Shot.Title,
					User = new UserShotCard
					{
						Username = item.Shot.User.Username,
						Image = item.ImageUrl
					},
					CountLike = await GetLikeCount(item.Shot.Id),
					CountView = await GetViewCount(item.Shot.Id),
					Image = item.ImageUrl
				});
			}

			return result;
		}

		public async Task<List<ShotLiked>> GetShotLikeds(Guid userId)
		{
			return await _context.Likes.Where(l => l.UserId == userId)
										.Select(l => new ShotLiked
										{
											Id = l.ShotId,
											Title = l.Shot.Title,
											Author = l.Shot.User.Username,
											Image = l.Shot.ImageVideos.Where(i => i.IsMain).Select(i => i.Url).FirstOrDefault() ?? "",
											DatePosted = l.CreateAt,
											Likes = _context.Likes.Count(like => like.ShotId == l.ShotId)
										}).ToListAsync();
		}
		public async Task<List<ShotView>> ListShotView(int page, int count)
		{
			// Truy vấn dữ liệu trước
			var shots = await _context.Shots
				.OrderByDescending(s => s.View)
				.Skip(page * count)
				.Take(count)
				.Include(s => s.User)
				.Include(s => s.ImageVideos)
				.ToListAsync();

			var shotViews = new List<ShotView>();

			foreach (var item in shots)
			{
				var countLikeTask = await GetLikeCount(item.Id);
				var countViewTask = await GetViewCount(item.Id);

				shotViews.Add(new ShotView
				{
					Id = item.Id,
					Title = item.Title,
					User = new UserShotCard
					{
						Username = item.User?.Username ?? "Unknown",
						Image = item.User?.ImageVideos?.FirstOrDefault()?.Url ?? ""
					},
					CountLike = countLikeTask,
					CountView = countViewTask,
					Image = item.ImageVideos?.Where(i => i.IsMain).Select(i => i.Url).FirstOrDefault() ?? ""
				});
			}

			return shotViews;
		}

	}
}
