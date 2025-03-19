using BE.src.api.domains.Database;
using BE.src.api.domains.Enum;
using BE.src.api.domains.DTOs.Post;
using BE.src.api.domains.Model;
using BE.src.api.helpers;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.UA;
using BE.src.api.domains.DTOs.ElasticSearch;

namespace BE.src.api.repositories
{
	public interface IPostRepo
	{
		Task<bool> CreatePost(PostJob post);
		Task<bool> CreateUserApply(UserApply userApply);
		Task<PostJob> GetPostJobById(Guid posId);
		Task<PostJob?> GetPostJobByCode(Guid code);
		Task<bool> IsApply(Guid UserId, Guid PostId);
		Task<bool> IsSaved(Guid UserId, Guid PostId);
		Task<List<UserApply>> UserApplyByPost(Guid PostId);
		Task<List<PostJob>> GetPosts(PostJobFilterDTO filter);
		Task<PostJob?> GetLatestPosts();
		Task<PostJob?> GetPostById(Guid id, CancellationToken cancellationToken = default);
		Task<List<PostJob>> GetAllPosts();
		Task<Attachment?> GetAttachmentById(Guid id);
		Task<List<PostJob>> GetListPost(int item, int page);
		Task<List<PostManageCard>> PostOwner(Guid userId);
		Task<List<FreelancerCard>> FreelancersByPost(Guid PostId);
		Task<PostManageCard?> GetPostEmployeeDetail(Guid PostId);
		Task<UserApply?> GetUserApply(Guid ApplyId);
		Task<bool> UpdateUserApply(UserApply userApply);
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

		public async Task<PostJob?> GetPostJobByCode(Guid code)
		{
			return await _context.PostJobs
										.Include(p => p.Attachments)
										.Include(p => p.CompanyLogo)
										.Include(p => p.Specialty)
										.FirstOrDefaultAsync(p =>
										p.Id == code);
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
						 .AsNoTracking()
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

		public async Task<List<PostJob>> GetAllPosts()
		{
			return await _context.PostJobs
						 .Include(p => p.User)
							 .ThenInclude(u => u.ImageVideos)
						 .ToListAsync();
		}

		public async Task<Attachment?> GetAttachmentById(Guid id)
		{
			return await _context.Attachments.FirstOrDefaultAsync(a => a.Id == id);
		}

		public async Task<List<PostJob>> GetListPost(int item, int page)
		{
			return await _context.PostJobs
							.Skip((page - 1) * item)
							.Take(item)
							.Include(p => p.CompanyLogo)
							.ToListAsync();
		}

		public async Task<List<PostManageCard>> PostOwner(Guid userId)
		{
			return await _context.PostJobs.Where(p => p.UserId == userId)
										.Select(s => new PostManageCard
										{
											Id = s.Id,
											Title = s.Title,
											Status = s.Status,
											WorkLocation = s.WorkLocation,
											WorkType = s.WorkType,
											EmploymentType = s.EmploymentType,
											CreateAt = s.CreateAt,
											CloseAt = s.CloseAt,
											NumberApplied = s.UserApplies.Count(),
											NumberHired = s.UserApplies.Count(ua => ua.Status == ApplyStatusEnum.Accept),
											Specialty = s.Specialty.Name
										})
										.ToListAsync();
		}

		public Task<List<FreelancerCard>> FreelancersByPost(Guid PostId)
		{
			return _context.UserApplies.Where(ua => ua.PostId == PostId)
										.Select(ua => new FreelancerCard
										{
											Username = ua.User.Username,
											Image = ua.User.ImageVideos.Select(i => i.Url).FirstOrDefault() ?? "",
											Place = ua.User.Place ?? "",
											Price = ua.User.Price ?? 0,
											Email = ua.User.Email,
											Status = ua.Status
										}).ToListAsync();
		}

		public async Task<PostManageCard?> GetPostEmployeeDetail(Guid PostId)
		{
			return await _context.PostJobs
				.Where(p => p.Id == PostId)
				.Select(s => new PostManageCard
				{
					Id = s.Id,
					Title = s.Title,
					Status = s.Status,
					WorkLocation = s.WorkLocation,
					WorkType = s.WorkType,
					EmploymentType = s.EmploymentType,
					CreateAt = s.CreateAt,
					CloseAt = s.CloseAt,
					NumberApplied = s.UserApplies.Count(),
					NumberHired = s.UserApplies.Count(ua => ua.Status == ApplyStatusEnum.Accept),
					Specialty = s.Specialty.Name
				})
				.FirstOrDefaultAsync();
		}

		public async Task<UserApply?> GetUserApply(Guid ApplyId)
		{
			return await _context.UserApplies.FirstOrDefaultAsync(a => a.Id == ApplyId);
		}

		public async Task<bool> UpdateUserApply(UserApply userApply)
		{
			_context.UserApplies.Update(userApply);
			return await _context.SaveChangesAsync() > 0;
		}
	}
}
