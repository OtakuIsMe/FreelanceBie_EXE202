using BE.src.api.domains.Database;
using BE.src.api.domains.Model;
using Microsoft.EntityFrameworkCore;

namespace BE.src.api.repositories
{
	public interface IUserRepo
	{
		Task<User?> GetUserByEmailPassword(string email, string password);
		Task<bool> CreateUser(User user);
		Task<List<User>> GetUsers();
		Task<User?> GetUserByEmail(string email);
		Task<bool> AddComment(Comment comment);
		Task<Follow?> GetFollow(Guid Follower, Guid Followed);
		Task<bool> DeleteFollow(Follow follow);
		Task<bool> CreateFollow(Follow follow);
		Task<Save?> GetUserSave(Guid userId);
		Task<bool> CreateSave(Save save);
		Task<bool> UpdateSave(Save save);
		Task<bool> DeleteSave(Save save);
	}
	public class UserRepo : IUserRepo
	{
		private readonly FLBDbContext _context;
		public UserRepo(FLBDbContext context)
		{
			_context = context;
		}

		public async Task<User?> GetUserByEmailPassword(string email, string password)
		{
			return await _context.Users.FirstOrDefaultAsync(u =>
								u.Email == email && u.Password == password);
		}
		public async Task<bool> CreateUser(User user)
		{
			await _context.Users.AddAsync(user);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<User?> GetUserByEmail(string email)
		{
			return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
		}

		public async Task<List<User>> GetUsers()
		{
			return await _context.Users.ToListAsync();
		}

		public async Task<bool> AddComment(Comment comment)
		{
			_context.Comments.Add(comment);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<Follow?> GetFollow(Guid Follower, Guid Followed)
		{
			return await _context.Follows.FirstOrDefaultAsync(f => f.FollowedId == Followed && f.FollowingId == Follower);
		}

		public async Task<bool> DeleteFollow(Follow follow)
		{
			_context.Follows.Remove(follow);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<bool> CreateFollow(Follow follow)
		{
			_context.Follows.Add(follow);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<Save?> GetUserSave(Guid userId)
		{
			return await _context.Saves.FirstOrDefaultAsync(s => s.UserId == userId);
		}

		public async Task<bool> CreateSave(Save save)
		{
			_context.Saves.Add(save);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<bool> UpdateSave(Save save)
		{
			_context.Saves.Update(save);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<bool> DeleteSave(Save save)
		{
			_context.Saves.Remove(save);
			return await _context.SaveChangesAsync() > 0;
		}
	}

}
