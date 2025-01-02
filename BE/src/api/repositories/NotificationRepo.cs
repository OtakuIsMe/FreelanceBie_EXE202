using BE.src.api.domains.Database;
using BE.src.api.domains.Model;
using Microsoft.EntityFrameworkCore;

namespace BE.src.api.repositories
{
    public interface INotificationRepo
    {
        public Task<List<Notification>> GetNotificationsByUserId(Guid userId);    
    }

	public class NotificationRepo : INotificationRepo
	{
        private readonly FLBDbContext _context;
        public NotificationRepo(FLBDbContext context)
        {
            _context = context;
        }
		public async Task<List<Notification>> GetNotificationsByUserId(Guid userId)
		{
			return await _context.Notifications.Where(x => x.UserId == userId)
                                                .ToListAsync();
		}
	}
}