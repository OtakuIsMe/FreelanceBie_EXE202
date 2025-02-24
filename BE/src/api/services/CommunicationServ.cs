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
        public CommunicationServ(ICommunicationRepo communicationRepo, IPostRepo postRepo)
        {
            _communicationRepo = communicationRepo;
            _postRepo = postRepo;
        }

		public async Task<IActionResult> GetAllCommunications(Guid userId)
		{
			try
            {
                var communications = await _communicationRepo.GetCommunications(userId);
                if (communications == null)
                {
                    return ErrorResp.NotFound("Communications not found");
                }
                return SuccessResp.Ok(communications);
            }
            catch (System.Exception ex)
            {
                return ErrorResp.BadRequest(ex.Message);
            }
		}

		public async Task<IActionResult> GetInTouch(Guid userId, Guid postId, string message)
		{
			try
			{
				var post = await _postRepo.GetPostById(postId);
				if (post == null)
				{
					return ErrorResp.NotFound("Post not found");
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
					return ErrorResp.BadRequest("Failed to send communication request");
				}

				var resultMessage = await _communicationRepo.AddMessage(messageContent);
				if (!resultMessage)
				{
					return ErrorResp.BadRequest("Failed to send message");
				}

				return SuccessResp.Created("Create Communication successfully");
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}

		public async Task<IActionResult> GetMessages(Guid communicationId)
		{
			try
            {
                var communication = await _communicationRepo.GetCommunicationById(communicationId);
                if (communication == null)
                {
                    return ErrorResp.NotFound("Communication not found");
                }

                var messages = await _communicationRepo.GetMessages(communicationId);
                if (messages == null)
                {
                    return ErrorResp.NotFound("Messages not found");
                }
                return SuccessResp.Ok(messages);
            }
            catch (System.Exception ex)
            {
                return ErrorResp.BadRequest(ex.Message);
            }
		}

		public async Task<IActionResult> SendMessage(Guid communicationId, Guid userId, string message)
		{
			try
			{
				var communication = await _communicationRepo.GetCommunicationById(communicationId);
				if (communication == null)
				{
					return ErrorResp.NotFound("Communication not found");
				}

                var messages = await _communicationRepo.GetMessages(communicationId);
                if (messages == null)
                {
                    return ErrorResp.NotFound("Messages not found");
                }

                var index = messages.Count;
                var isZero = communication.ZeroId == userId ? true : false;

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
					return ErrorResp.BadRequest("Failed to send message");
				}

				return SuccessResp.Created("Message sent successfully");
			}
			catch (System.Exception ex)
			{
				return ErrorResp.BadRequest(ex.Message);
			}
		}
    }
}