using BE.src.api.domains.Database;
using BE.src.api.domains.Enum;
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

	}
}
