using BE.src.api.domains.Database;
using BE.src.api.domains.Model;
using Microsoft.EntityFrameworkCore;

namespace BE.src.api.repositories
{
	public interface IPostRepo
	{
		Task<bool> CreatePost(PostJob post);
		Task<bool> CreateUserApply(UserApply userApply);
		Task<PostJob> GetPostJobById(Guid posId);
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

		public async Task<PostJob> GetPostJobById(Guid posId)
		{
			return await _context.PostJobs.FirstAsync(p => p.Id == posId);
		}
	}
}
