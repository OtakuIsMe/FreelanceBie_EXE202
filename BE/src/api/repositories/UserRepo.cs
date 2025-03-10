using BE.src.api.domains.Database;
using BE.src.api.domains.DTOs.ElasticSearch;
using BE.src.api.domains.DTOs.User;
using BE.src.api.domains.Enum;
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
		Task<Save?> GetUserSave(Guid userId, CancellationToken cancellationToken = default);
		Task<bool> CreateSave(Save save);
		Task<bool> UpdateSave(Save save);
		Task<bool> DeleteSave(Save save);
		Task<bool> ChangePassword(string email, string newPassword);
		Task<User?> ViewProfileUser(Guid userId, CancellationToken cancellationToken = default);
		Task<User?> GetUserById(Guid userId, CancellationToken cancellationToken = default);
		Task<bool> EditProfile(User user);
		Task<List<User>> FindUsers(UserSearchingDTO userSearchingDTO);
		Task<bool> AddNewRefreshToken(RefreshToken refreshToken);
		Task<RefreshToken?> GetRefreshToken(string refreshToken);
		Task<bool> UpdateNewRefreshToken(RefreshToken refreshToken);
		Task<bool> RevokeRefreshToken(Guid userId);
		Task<List<RefreshToken>> GetRefreshTokens(Guid userId);
		Task<List<User>> GetOnlyCustomers();
		Task<bool> AddImageVideo(ImageVideo img);
		Task<bool> UpdateImageVideo(ImageVideo img);
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

		public async Task<Save?> GetUserSave(Guid userId, CancellationToken cancellationToken = default)
		{
			return await _context.Saves.FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
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

		public async Task<bool> ChangePassword(string email, string newPassword)
		{
			var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
			if (user == null) return false;

			user.Password = newPassword;
			_context.Users.Update(user);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<User?> ViewProfileUser(Guid userId, CancellationToken cancellationToken = default)
		{
			return await _context.Users
								.Include(x => x.ImageVideos)
								.Include(x => x.Comments)
								.Include(x => x.Likes)
								.Include(x => x.Saves)
									.ThenInclude(x => x.Post)
								.Include(x => x.Saves)
									.ThenInclude(x => x.Shot)
										.ThenInclude(x => x.ImageVideos)
								.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
		}

		public async Task<bool> EditProfile(User user)
		{
			_context.Users.Update(user);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> AddImageVideo(ImageVideo img)
		{
			_context.ImageVideos.Add(img);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<bool> UpdateImageVideo(ImageVideo img)
		{
			_context.ImageVideos.Update(img);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<User?> GetUserById(Guid userId, CancellationToken cancellationToken = default)
		{
			return await _context.Users.Include(x => x.ImageVideos).FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
		}

		public async Task<List<User>> FindUsers(UserSearchingDTO userSearchingDTO)
		{
			return await _context.Users.Where(x => x.Role == RoleEnum.Customer &&
				(string.IsNullOrEmpty(userSearchingDTO.Name) || x.Name.ToLower().Contains(userSearchingDTO.Name.ToLower())) &&
				(string.IsNullOrEmpty(userSearchingDTO.Username) || x.Username.ToLower().Contains(userSearchingDTO.Username.ToLower())) &&
				(string.IsNullOrEmpty(userSearchingDTO.Email) || x.Email.ToLower().Contains(userSearchingDTO.Email.ToLower())) &&
				(string.IsNullOrEmpty(userSearchingDTO.Phone) || x.Phone.Contains(userSearchingDTO.Phone)) &&
				(string.IsNullOrEmpty(userSearchingDTO.City) || x.City.ToLower().Contains(userSearchingDTO.City.ToLower())) &&
				(string.IsNullOrEmpty(userSearchingDTO.Education) || x.Education.ToLower().Contains(userSearchingDTO.Education.ToLower()))
			)
			.Include(x => x.ImageVideos)
			.ToListAsync();
		}

		public async Task<bool> AddNewRefreshToken(RefreshToken refreshToken)
		{
			await _context.RefreshTokens.AddAsync(refreshToken);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<RefreshToken?> GetRefreshToken(string refreshToken)
		{
			return await _context.RefreshTokens
							.Include(r => r.User)
							.FirstOrDefaultAsync(x => x.Token == refreshToken);
		}

		public async Task<bool> UpdateNewRefreshToken(RefreshToken refreshToken)
		{
			_context.RefreshTokens.Update(refreshToken);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<bool> RevokeRefreshToken(Guid userId)
		{
			await _context.RefreshTokens.Where(u => u.UserId == userId).ExecuteDeleteAsync();

			return true;
		}

		public async Task<List<RefreshToken>> GetRefreshTokens(Guid userId)
		{
			return await _context.RefreshTokens.Where(rft => rft.UserId == userId).ToListAsync();
		}

		public async Task<List<User>> GetOnlyCustomers()
		{
			return await _context.Users
								.Where(user => user.Role == RoleEnum.Customer)
								.Include(x => x.ImageVideos)
								.ToListAsync();
		}
	}
}
