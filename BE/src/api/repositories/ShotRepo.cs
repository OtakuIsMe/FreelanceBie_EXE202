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
	}
}
