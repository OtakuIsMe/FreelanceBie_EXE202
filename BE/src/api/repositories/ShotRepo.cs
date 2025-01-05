using BE.src.api.domains.Database;
using BE.src.api.domains.DTOs.Shot;
using BE.src.api.domains.Model;
using Microsoft.EntityFrameworkCore;

namespace BE.src.api.repositories
{
	public interface IShotRepo
	{
		Task<bool> CreateShot(Shot shot);
		Task<Like?> GetLike(Guid userId, Guid shotId);
		Task<bool> CreateLikeShot(Like like);
		Task<bool> DeleteLikeShot(Like like);
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

		public async Task<List<Shot>> GetShots(ShotSearchFilterDTO filter)
		{
			var query = _context.Shots
							.Include(s => s.User)      
							.Include(s => s.Specialty)
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
				query = query.Where(s => s.Specialty.Name.Contains(filter.SpecialtyName));

			if (!string.IsNullOrEmpty(filter.HtmlKeyword))
				query = query.Where(s => s.Html.Contains(filter.HtmlKeyword));

			if (!string.IsNullOrEmpty(filter.CssKeyword))
				query = query.Where(s => s.Css.Contains(filter.CssKeyword));

			if (filter.MinViews.HasValue)
				query = query.Where(s => s.View >= filter.MinViews);

			if (filter.MaxViews.HasValue)
				query = query.Where(s => s.View <= filter.MaxViews);

			return await query.ToListAsync();
		}
	}
}
