using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.shared.Type;
using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.services
{
    public interface INotificationServ
    {
        Task<IActionResult> ViewNotifications(Guid userId);
    }
	public class NotificationServ : INotificationServ
	{
        private readonly INotificationRepo _notificationRepo;
        private readonly ICacheService _cacheService;
        public NotificationServ(INotificationRepo notificationRepo, ICacheService cacheService)
        {
            _notificationRepo = notificationRepo;
            _cacheService = cacheService;
        }
		public async Task<IActionResult> ViewNotifications(Guid userId)
		{
			try
            {
                var cachedNotifications = await _cacheService.Get<List<Notification>>($"notifications-{userId}");
                if (cachedNotifications != null)
                    return SuccessResp.Ok(cachedNotifications);

                var notifications = await _notificationRepo.GetNotificationsByUserId(userId);
                if(notifications.Count == 0)
                    return ErrorResp.NotFound("Notifications not found");

                await _cacheService.Set($"notifications-{userId}", notifications, TimeSpan.FromMinutes(10));

                return SuccessResp.Ok(notifications);
            }
            catch (System.Exception ex)
            {
                return ErrorResp.BadRequest(ex.Message);
            }
		}
	}
}