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
		Task<bool> ChangePassword(string email, string newPassword);
		Task<User?> ViewProfileUser(Guid userId);
		Task<User?> GetUserById(Guid userId);
		Task<bool> EditProfile(User user);
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

		public async Task<bool> ChangePassword(string email, string newPassword)
		{
			var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
			if (user == null) return false;

			user.Password = newPassword;
			_context.Users.Update(user);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<User?> ViewProfileUser(Guid userId)
		{
			return await _context.Users
								.Include(x => x.ImageVideos)
								.Include(x => x.Notifications)
								.Include(x => x.SocialProfiles)
								.Include(x => x.Comments)
								.Include(x => x.Likes)
									.ThenInclude(x => x.User)
										.ThenInclude(x => x.ImageVideos)
								.Include(x => x.Likes)
									.ThenInclude(x => x.User)
										.ThenInclude(x => x.Shots)
											.ThenInclude(x => x.ImageVideos)
								.Include(x => x.Saves)
									.ThenInclude(x => x.Shot)
										.ThenInclude(x => x.ImageVideos)
								.Include(x => x.Saves)
									.ThenInclude(x => x.User)
										.ThenInclude(x => x.ImageVideos)
								.Include(x => x.Saves)
									.ThenInclude(x => x.Post)
								.Include(x => x.Saves)
									.ThenInclude(x => x.Shot)
										.ThenInclude(x => x.ImageVideos)
								.FirstOrDefaultAsync(x => x.Id == userId);
		}

		public async Task<bool> EditProfile(User user)
		{
			_context.Users.Update(user);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<User?> GetUserById(Guid userId)
		{
			return await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
		}
	}
}
