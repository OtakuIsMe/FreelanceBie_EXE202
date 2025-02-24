using BE.src.api.domains.Database;
using BE.src.api.domains.Enum;
using BE.src.api.domains.DTOs.Post;
using BE.src.api.domains.Model;
using BE.src.api.helpers;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.UA;

namespace BE.src.api.repositories
{
	public interface IPostRepo
	{
		Task<bool> CreatePost(PostJob post);
		Task<bool> CreateUserApply(UserApply userApply);
		Task<PostJob> GetPostJobById(Guid posId);
		Task<PostJob?> GetPostJobByCode(string code);
		Task<bool> IsApply(Guid UserId, Guid PostId);
		Task<bool> IsSaved(Guid UserId, Guid PostId);
		Task<List<UserApply>> UserApplyByPost(Guid PostId);
		Task<List<PostJob>> GetPosts(PostJobFilterDTO filter);
		Task<PostJob?> GetLatestPosts();
		Task<PostJob?> GetPostById(Guid id, CancellationToken cancellationToken = default);
	}
	public class PostRepo : IPostRepo
	{
		private readonly FLBDbContext _context;
		public PostRepo(FLBDbContext context)
		{
			_context = context;
		}
		public async Task<bool> CreatePost(PostJob post)
		{
			_context.PostJobs.Add(post);
			return await _context.SaveChangesAsync() > 0;
		}
		public async Task<bool> CreateUserApply(UserApply userApply)
		{
			_context.UserApplies.Add(userApply);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<PostJob?> GetPostJobByCode(string code)
		{
			return await _context.PostJobs
										.Include(p => p.Attachments)
										.Include(p => p.CompanyLogo)
										.Include(p => p.Specialty)
										.FirstOrDefaultAsync(p =>
										Utils.HashObject(p.Id) == code);
		}

		public async Task<PostJob> GetPostJobById(Guid posId)
		{
			return await _context.PostJobs.FirstAsync(p => p.Id == posId);
		}

		public async Task<bool> IsApply(Guid UserId, Guid PostId)
		{
			return await _context.UserApplies.AnyAsync(ua => ua.UserId == UserId
											&& ua.PostId == PostId);
		}

		public async Task<bool> IsSaved(Guid UserId, Guid PostId)
		{
			return await _context.Saves.AnyAsync(s => s.UserId == UserId
										&& s.PostId == PostId);
		}

		public async Task<List<UserApply>> UserApplyByPost(Guid PostId)
		{
			return await _context.UserApplies.Where(ua => ua.PostId == PostId
										&& ua.Status == ApplyStatusEnum.Accept)
										.Include(ua => ua.User)
										.ToListAsync();
		}


		public async Task<List<PostJob>> GetPosts(PostJobFilterDTO filter)
		{
			var query = _context.PostJobs
								.Include(p => p.User)
									.ThenInclude(u => u.ImageVideos)
								.Include(p => p.Specialty)
								.AsQueryable();

			if (!string.IsNullOrEmpty(filter.Title))
				query = query.Where(p => p.Title.Contains(filter.Title));

			if (filter.WorkType.HasValue)
				query = query.Where(p => p.WorkType == filter.WorkType);

			if (!string.IsNullOrEmpty(filter.WorkLocation))
				query = query.Where(p => p.WorkLocation.Contains(filter.WorkLocation));

			if (!string.IsNullOrEmpty(filter.CompanyName))
				query = query.Where(p => p.CompanyName.Contains(filter.CompanyName));

			if (filter.EmploymentType.HasValue)
				query = query.Where(p => p.EmploymentType == filter.EmploymentType);

			if (filter.MinExperience.HasValue)
				query = query.Where(p => p.Experience >= filter.MinExperience);

			if (filter.MaxExperience.HasValue)
				query = query.Where(p => p.Experience <= filter.MaxExperience);

			if (!string.IsNullOrEmpty(filter.UserName))
				query = query.Where(p => p.User.Username.Contains(filter.UserName));

			if (!string.IsNullOrEmpty(filter.UserEmail))
				query = query.Where(p => p.User.Email.Contains(filter.UserEmail));

			if (!string.IsNullOrEmpty(filter.SpecialtyName))
				query = query.Where(p => p.Specialty.Name.Contains(filter.SpecialtyName));

			return await query.ToListAsync();
		}

		public async Task<PostJob?> GetLatestPosts()
		{
			return await _context.PostJobs
						 .Include(p => p.User)
							 .ThenInclude(u => u.ImageVideos)
						 .OrderByDescending(p => p.CreateAt)
						 .FirstOrDefaultAsync();
		}

		public async Task<PostJob?> GetPostById(Guid id, CancellationToken cancellationToken = default)
		{
			return await _context.PostJobs
						 .Include(p => p.User)
							 .ThenInclude(u => u.ImageVideos)
						 .Include(p => p.Specialty)
						 .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
		}
	}
}
