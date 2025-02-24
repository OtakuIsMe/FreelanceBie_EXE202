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
		Task<Shot?> GetShotByShotCode(string shotCode);
		Task<bool> IsLikedShot(Guid UserId, Guid ShotId);
		Task<bool> IsSaved(Guid UserId, Guid ShotId);
		Task<List<Shot>> GetShots(ShotSearchFilterDTO filter);
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
										.Include(s => s.ImageVideos)
										.Include(s => s.Specialties)
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

		public async Task<Shot?> GetShotByShotCode(string shotCode)
		{
			return await _context.Shots
						.Include(s => s.User)
							.ThenInclude(u => u.ImageVideos)
						.FirstOrDefaultAsync(s => Utils.HashObject<Guid>(s.Id) == shotCode);
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
	}
}
