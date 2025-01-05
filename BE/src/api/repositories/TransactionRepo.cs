using BE.src.api.domains.Database;
using BE.src.api.domains.Model;

namespace BE.src.api.repositories
{
    public interface ITransactionRepo
    {
        Task<bool> CreateTransaction(Transaction transaction);
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
	}
}