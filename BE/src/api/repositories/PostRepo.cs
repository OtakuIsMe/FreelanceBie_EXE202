using BE.src.api.domains.Database;
using BE.src.api.domains.Model;

namespace BE.src.api.repositories
{
	public interface IPostRepo
	{
		Task<bool> CreatePost(PostJob post);
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
	}
}
