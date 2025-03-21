using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.shared.Type;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Org.BouncyCastle.Asn1.Cms;

namespace BE.src.api.services
{
	public interface ICommunicationServ
	{
		Task<IActionResult> GetAllCommunications(Guid userId);
		Task<IActionResult> GetMessages(Guid communicationId, Guid userId);
		Task<IActionResult> GetInTouch(Guid userId, Guid ownerId);
		// Task<IActionResult> SendMessage(Guid communicationId, Guid userId, string message);
	}
	public class CommunicationServ : ICommunicationServ
	{
		private readonly ICommunicationRepo _communicationRepo;
		private readonly ICacheService _cacheService;
		public CommunicationServ(ICommunicationRepo communicationRepo, ICacheService cacheService)
		{
			_communicationRepo = communicationRepo;
			_cacheService = cacheService;
		}

		public async Task<IActionResult> GetAllCommunications(Guid userId)
		{
			try
			{
				// string key = $"communications:{userId}";

				// var cachedCommunications = await _cacheService.Get<List<Communication>>(key);
				// if (cachedCommunications != null)
				//    return SuccessResp.Ok(cachedCommunications);

				var communications = await _communicationRepo.GetCommunications(userId);
				// if (communications == null)
				// {
				//     throw new ApplicationException("Communications not found");
				// }

				// await _cacheService.Set(key, communications, TimeSpan.FromMinutes(10));

				return SuccessResp.Ok(communications);
			}
			catch (System.Exception ex)
			{
				throw new ApplicationException(ex.Message);
			}
		}

		public async Task<IActionResult> GetInTouch(Guid userId, Guid ownerId)
		{
			try
			{
				var communication = await _communicationRepo.GetCommunicationBy2Person(userId, ownerId);
				if (communication == null)
				{
					communication = new Communication
					{
						ZeroId = userId,
						FirstId = ownerId,
					};

					var result = await _communicationRepo.AddCommunication(communication);
					if (!result)
					{
						throw new ApplicationException("Failed to send communication request");
					}
				}
				return SuccessResp.Created(new
				{
					communicationId = communication.Id
				});
			}
			catch (System.Exception ex)
			{
				throw new ApplicationException(ex.Message);
			}
		}

		public async Task<IActionResult> GetMessages(Guid communicationId, Guid userId)
		{
			try
			{
				// 	var key = $"messages:{communicationId}";

				// 	var cachedMessages = await _cacheService.Get<List<Message>>(key);
				// 	if (cachedMessages != null)
				// 		return SuccessResp.Ok(cachedMessages);

				// 	var communication = await _communicationRepo.GetCommunicationById(communicationId);
				// 	if (communication == null)
				// 	{
				// 		throw new ApplicationException("Communication not found");
				// 	}

				var messages = await _communicationRepo.GetMessages(communicationId, userId);
				// if (messages == null || !messages.Any())
				// 	throw new ApplicationException("Messages not found");

				// await _cacheService.Set(key, messages, TimeSpan.FromMinutes(10));

				return SuccessResp.Ok(messages);
			}
			catch (System.Exception ex)
			{
				throw new ApplicationException(ex.Message);
			}
		}

		// public async Task<IActionResult> SendMessage(Guid communicationId, Guid userId, string message)
		// {
		// 	try
		// 	{
		// 		var communication = await _communicationRepo.GetCommunicationById(communicationId);
		// 		if (communication == null)
		// 		{
		// 			throw new ApplicationException("Communication not found");
		// 		}

		// 		var messages = await _communicationRepo.GetMessages(communicationId);
		// 		if (messages == null)
		// 		{
		// 			throw new ApplicationException("Messages not found");
		// 		}

		// 		var index = messages.Count;
		// 		var isZero = communication.ZeroId == userId;

		// 		var messageContent = new Message
		// 		{
		// 			CommunicationId = communication.Id,
		// 			Content = message,
		// 			PersonIndex = isZero,
		// 			Index = index++,
		// 		};

		// 		var result = await _communicationRepo.AddMessage(messageContent);
		// 		if (!result)
		// 		{
		// 			throw new ApplicationException("Failed to send message");
		// 		}

		// 		var key = $"messages:{communicationId}";
		// 		await _cacheService.Remove(key);

		// 		return SuccessResp.Created("Message sent successfully");
		// 	}
		// 	catch (System.Exception ex)
		// 	{
		// 		throw new ApplicationException(ex.Message);
		// 	}
		// }
	}
}
