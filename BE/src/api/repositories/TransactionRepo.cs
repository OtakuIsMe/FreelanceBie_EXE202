using BE.src.api.domains.Database;
using BE.src.api.domains.Model;
using Microsoft.EntityFrameworkCore;

namespace BE.src.api.repositories
{
    public interface ITransactionRepo
    {
        Task<bool> CreateTransaction(Transaction transaction);
        Task<List<Transaction>> GetTransactions(Guid userId);
    }
    public class TransactionRepo : ITransactionRepo
    {
        private readonly FLBDbContext _context;
        public TransactionRepo(FLBDbContext context)
        {
            _context = context;
        }

		public async Task<bool> CreateTransaction(Transaction transaction)
		{
			await _context.Transactions.AddAsync(transaction);
            return await _context.SaveChangesAsync() > 0;
		}

		public async Task<List<Transaction>> GetTransactions(Guid userId)
		{
			return await _context.Transactions.Where(t => t.MemberUser.UserId == userId)
                                                .Include(t => t.MemberUser)
                                                    .ThenInclude(mu => mu.User)
                                                       .ThenInclude(u => u.ImageVideos)
                                                .ToListAsync();
		}
	}
}