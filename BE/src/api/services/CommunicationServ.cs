using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.shared.Type;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Cms;

namespace BE.src.api.services
{
    public interface ICommunicationServ
    {
        Task<IActionResult> GetAllCommunications(Guid userId);
        Task<IActionResult> GetMessages(Guid communicationId);
        Task<IActionResult> GetInTouch(Guid userId, Guid postId, string message);
        Task<IActionResult> SendMessage(Guid communicationId, Guid userId, string message);
    }
    public class CommunicationServ : ICommunicationServ
    {
        private readonly ICommunicationRepo _communicationRepo;
        private readonly IPostRepo _postRepo;
		private readonly ICacheService _cacheService;
        public CommunicationServ(ICommunicationRepo communicationRepo, IPostRepo postRepo, ICacheService cacheService)
        {
            _communicationRepo = communicationRepo;
            _postRepo = postRepo;
			_cacheService = cacheService;
        }

		public async Task<IActionResult> GetAllCommunications(Guid userId)
		{
			try
            {
				string key = $"communications:{userId}";

				var cachedCommunications = await _cacheService.Get<List<Communication>>(key);
				if (cachedCommunications != null)
				   return SuccessResp.Ok(cachedCommunications);

                var communications = await _communicationRepo.GetCommunications(userId);
                if (communications == null)
                {
                    throw new ApplicationException("Communications not found");
                }
                
				await _cacheService.Set(key, communications, TimeSpan.FromMinutes(10));

				return SuccessResp.Ok(communications);
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
		}

		public async Task<IActionResult> GetInTouch(Guid userId, Guid postId, string message)
		{
			try
			{
				var post = await _postRepo.GetPostById(postId);
				if (post == null)
				{
					throw new ApplicationException("Post not found");
				}

				var communication = new Communication
				{
					ZeroId = userId,
					FirstId = post.UserId,
				};

				var messageContent = new Message
				{
					CommunicationId = communication.Id,
					Content = message,
					PersonIndex = true,
					Index = 0,
				};

				var result = await _communicationRepo.AddCommunication(communication);
				if (!result)
				{
					throw new ApplicationException("Failed to send communication request");
				}

				var resultMessage = await _communicationRepo.AddMessage(messageContent);
				if (!resultMessage)
				{
					throw new ApplicationException("Failed to send message");
				}

				return SuccessResp.Created("Create Communication successfully");
			}
			catch (System.Exception ex)
			{
				throw new ApplicationException(ex.Message);
			}
		}

		public async Task<IActionResult> GetMessages(Guid communicationId)
		{
			try
            {
				var key = $"messages:{communicationId}";

				var cachedMessages = await _cacheService.Get<List<Message>>(key);
				if (cachedMessages != null)
				   return SuccessResp.Ok(cachedMessages);

                var communication = await _communicationRepo.GetCommunicationById(communicationId);
                if (communication == null)
                {
                    throw new ApplicationException("Communication not found");
                }

                var messages = await _communicationRepo.GetMessages(communicationId);
                if (messages == null || !messages.Any())
            		throw new ApplicationException("Messages not found");

				await _cacheService.Set(key, messages, TimeSpan.FromMinutes(10));

                return SuccessResp.Ok(messages);
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
		}

		public async Task<IActionResult> SendMessage(Guid communicationId, Guid userId, string message)
		{
			try
			{
				var communication = await _communicationRepo.GetCommunicationById(communicationId);
				if (communication == null)
				{
					throw new ApplicationException("Communication not found");
				}

                var messages = await _communicationRepo.GetMessages(communicationId);
                if (messages == null)
                {
                    throw new ApplicationException("Messages not found");
                }

                var index = messages.Count;
                var isZero = communication.ZeroId == userId;

				var messageContent = new Message
				{
					CommunicationId = communication.Id,
					Content = message,
					PersonIndex = isZero,
					Index = index++,
				};

				var result = await _communicationRepo.AddMessage(messageContent);
				if (!result)
				{
					throw new ApplicationException("Failed to send message");
				}

				var key = $"messages:{communicationId}";
				await _cacheService.Remove(key);

				return SuccessResp.Created("Message sent successfully");
			}
			catch (System.Exception ex)
			{
				throw new ApplicationException(ex.Message);
			}
		}
    }
}