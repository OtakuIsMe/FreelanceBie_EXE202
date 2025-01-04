using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public NotificationServ(INotificationRepo notificationRepo)
        {
            _notificationRepo = notificationRepo;
        }
		public async Task<IActionResult> ViewNotifications(Guid userId)
		{
			try
            {
                var notifications = await _notificationRepo.GetNotificationsByUserId(userId);
                if(notifications.Count == 0)
                {
                    return ErrorResp.NotFound("No notifications found");
                }
                return SuccessResp.Ok(notifications);
            }
            catch (System.Exception ex)
            {
                return ErrorResp.BadRequest(ex.Message);
            }
		}
	}
}