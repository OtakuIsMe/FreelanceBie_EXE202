using BE.src.api.controllers;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using Microsoft.AspNetCore.SignalR;
using Nest;

namespace BE.src.api.services
{
	public interface IChatServ
	{
		Task SendMessage(Guid CommunicationId, Guid SenderId, string message);
	}
	public class ChatServ : IChatServ
	{
		private readonly ICommunicationRepo _communicationRepo;
		private readonly IHubContext<ChatHub> _hubContext;
		public ChatServ(ICommunicationRepo communicationRepo, IHubContext<ChatHub> hubContext)
		{
			_communicationRepo = communicationRepo;
			_hubContext = hubContext;
		}
		public async Task SendMessage(Guid CommunicationId, Guid SenderId, string message)
		{
			try
			{
				var communication = await _communicationRepo.GetCommunicationById(CommunicationId);

				if (communication == null)
				{
					throw new Exception("Communication not found");
				}

				Message? messageObj;
				if (communication.ZeroId == SenderId)
				{
					messageObj = new Message
					{
						CommunicationId = communication.Id,
						Content = message,
						Index = 0,
						PersonIndex = false
					};
				}
				else if (communication.FirstId == SenderId)
				{
					messageObj = new Message
					{
						CommunicationId = communication.Id,
						Content = message,
						Index = 0,
						PersonIndex = true
					};
				}
				else
				{
					throw new Exception("Cant find this user");
				}
				await _communicationRepo.AddMessage(messageObj);
				await _hubContext.Clients.Group(CommunicationId.ToString())
					.SendAsync("ReceiveMessage", message, SenderId, messageObj.CreateAt);
			}
			catch (System.Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
	}
}
