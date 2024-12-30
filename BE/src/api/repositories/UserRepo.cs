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
	}

}
