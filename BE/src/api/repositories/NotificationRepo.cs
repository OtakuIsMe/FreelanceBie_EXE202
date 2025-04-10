using BE.src.api.domains.Database;
using BE.src.api.domains.Model;
using Microsoft.EntityFrameworkCore;

namespace BE.src.api.repositories
{
	public interface INotificationRepo
	{
		public Task<List<Notification>> GetNotificationsByUserId(Guid userId);
		Task<bool> CreateNotification(Notification notification);
		Task<Notification?> GetNotificationByPostId(Guid PostId);
		Task<bool> UpdateNotification(Notification notification);
		Task<bool> AddNotification(Notification notification);
		Task<bool> CountExistingNotification(Guid PostId, Guid UserId);
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

		public async Task<bool> AddNotification(Notification notification)
		{
			await _context.Notifications.AddAsync(notification);
			return await _context.SaveChangesAsync() > 0;
		}
		public async Task<bool> CreateNotification(Notification notification)
		{
			_context.Notifications.Add(notification);
			return await _context.SaveChangesAsync() > 0;
		}
		public async Task<Notification?> GetNotificationByPostId(Guid PostId)
		{
			return await _context.Notifications.Include(notify => notify.Post).FirstOrDefaultAsync(n => n.PostId == PostId);
		}
		public async Task<bool> UpdateNotification(Notification notification)
		{
			_context.Notifications.Update(notification);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<bool> CountExistingNotification(Guid PostId, Guid UserId)
		{
			return await _context.Notifications.AnyAsync(x => x.PostId == PostId && x.UserId == UserId);
		}
	}
}
