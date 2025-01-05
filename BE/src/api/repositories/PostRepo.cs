using BE.src.api.domains.Database;
using BE.src.api.domains.DTOs.Post;
using BE.src.api.domains.Model;
using Microsoft.EntityFrameworkCore;

namespace BE.src.api.repositories
{
	public interface IPostRepo
	{
		Task<bool> CreatePost(PostJob post);
		Task<List<PostJob>> GetPosts(PostJobFilterDTO filter);
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
	}
}
