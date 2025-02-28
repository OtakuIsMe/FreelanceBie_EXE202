using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.src.api.domains.Database;
using BE.src.api.domains.Model;
using Microsoft.EntityFrameworkCore;

namespace BE.src.api.repositories
{
    public interface ICommunicationRepo
    {
        Task<List<Communication>> GetCommunications(Guid userId);
        Task<List<Message>> GetMessages(Guid communicationId);
        Task<bool> AddCommunication(Communication communication);
        Task<bool> AddMessage(Message message);
        Task<Communication?> GetCommunicationById(Guid id, CancellationToken cancellationToken = default);
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

		public async Task<Communication?> GetCommunicationById(Guid id, CancellationToken cancellationToken = default)
		{
			return await _context.Communications.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
		}

		public async Task<List<Communication>> GetCommunications(Guid userId)
        {
            return await _context.Communications
                                .Where(x => x.ZeroId == userId || x.FirstId == userId)
                                .Include(x => x.Messages)
                                .ToListAsync();
        }

		public Task<List<Message>> GetMessages(Guid communicationId)
		{
			return _context.Messages
                            .Where(x => x.CommunicationId == communicationId)
                            .ToListAsync();
		}
	}
}