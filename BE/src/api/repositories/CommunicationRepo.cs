using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.src.api.domains.Database;
using BE.src.api.domains.DTOs.User;
using BE.src.api.domains.Model;
using Microsoft.EntityFrameworkCore;

namespace BE.src.api.repositories
{
	public interface ICommunicationRepo
	{
		Task<List<CommunicationDTO>> GetCommunications(Guid userId);
		Task<MessagesDTO?> GetMessages(Guid communicationId, Guid userId);
		Task<bool> AddCommunication(Communication communication);
		Task<bool> AddMessage(Message message);
		Task<Communication?> GetCommunicationById(Guid id, CancellationToken cancellationToken = default);
		Task<Communication?> GetCommunicationBy2Person(Guid zero, Guid first);
	}
	public class CommunicationRepo : ICommunicationRepo
	{
		private readonly FLBDbContext _context;
		public CommunicationRepo(FLBDbContext context)
		{
			_context = context;
		}

		public async Task<bool> AddCommunication(Communication communication)
		{
			await _context.Communications.AddAsync(communication);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<bool> AddMessage(Message message)
		{
			await _context.Messages.AddAsync(message);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<Communication?> GetCommunicationBy2Person(Guid zero, Guid first)
		{
			return await _context.Communications.FirstOrDefaultAsync(c => (c.ZeroId == zero && c.FirstId == first) || (c.ZeroId == first && c.FirstId == zero));
		}

		public async Task<Communication?> GetCommunicationById(Guid id, CancellationToken cancellationToken = default)
		{
			return await _context.Communications.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
		}

		public async Task<List<CommunicationDTO>> GetCommunications(Guid userId)
		{
			return await _context.Communications
				.Where(c => c.ZeroId == userId || c.FirstId == userId)
				.Select(c => new
				{
					c.Id,
					Username = c.ZeroId == userId ? c.First.Username : c.Zero.Username,
					UserImage = c.ZeroId == userId
						? c.First.ImageVideos.Select(i => i.Url).FirstOrDefault()
						: c.Zero.ImageVideos.Select(i => i.Url).FirstOrDefault(),
					LastMessage = c.Messages.OrderByDescending(m => m.CreateAt).FirstOrDefault()
				})
				.Select(c => new CommunicationDTO
				{
					MessageId = c.Id,
					Username = c.Username,
					UserImage = c.UserImage ?? "",
					LastMessage = c.LastMessage != null ? c.LastMessage.Content : "",
					LastMessageTime = c.LastMessage != null ? c.LastMessage.CreateAt : null
				})
				.ToListAsync();
		}

		public async Task<MessagesDTO?> GetMessages(Guid communicationId, Guid userId)
		{

			var communication = await _context.Communications
				.Where(c => c.Id == communicationId)
				.Select(c => new MessagesDTO
				{
					Self = c.ZeroId == userId ? new UserMes
					{
						Id = c.ZeroId,
						Username = c.Zero.Username,
						UserImage = c.Zero.ImageVideos.Select(i => i.Url).FirstOrDefault() ?? ""
					} : new UserMes
					{
						Id = c.FirstId,
						Username = c.First.Username,
						UserImage = c.First.ImageVideos.Select(i => i.Url).FirstOrDefault() ?? ""
					},
					Partner = c.ZeroId != userId ? new UserMes
					{
						Id = c.ZeroId,
						Username = c.Zero.Username,
						UserImage = c.Zero.ImageVideos.Select(i => i.Url).FirstOrDefault() ?? ""
					} : new UserMes
					{
						Id = c.FirstId,
						Username = c.First.Username,
						UserImage = c.First.ImageVideos.Select(i => i.Url).FirstOrDefault() ?? ""
					},
					Messages = c.Messages
						.OrderBy(m => m.CreateAt)
						.Select(m => new Mes
						{
							IsSelf = (c.ZeroId == userId) == !m.PersonIndex,
							Message = m.Content,
							CreateAt = m.CreateAt
						}).ToList()
				})
				.FirstOrDefaultAsync();

			return communication;
		}
	}
}
