using BE.src.api.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BE.src.api.controllers
{
	public class ChatHub : Hub
	{
		private readonly IChatServ _chatServ;
		private ILogger<ChatHub> _logger;
		public ChatHub(IChatServ chatServ, ILogger<ChatHub> logger)
		{
			_chatServ = chatServ;
			_logger = logger;
		}
		public async Task SendMessage(Guid CommunicationId, string message)
		{
			var senderIdClaim = Context.User?.Claims.FirstOrDefault(u => u.Type == "userId");

			if (senderIdClaim == null || string.IsNullOrEmpty(senderIdClaim.Value))
			{
				throw new HubException("Unauthorized: Missing userId claim.");
			}

			Guid SenderId = Guid.Parse(senderIdClaim.Value);
			await _chatServ.SendMessage(CommunicationId, SenderId, message);
		}

		public async Task JoinGroup(string CommunicationId)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, CommunicationId);
		}
	}
}
