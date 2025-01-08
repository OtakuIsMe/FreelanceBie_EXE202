using BE.src.api.domains.Database;
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
		Task<List<Shot>> GetShotsByUser(Guid userId);
		Task<int> GetLikeCount(Guid ShotId);
		Task<int> GetViewCount(Guid ShotId);
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
	}
}
