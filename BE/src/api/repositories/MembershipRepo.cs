using BE.src.api.domains.Database;
using BE.src.api.domains.Model;
using Microsoft.EntityFrameworkCore;

namespace BE.src.api.repositories
{
    public interface IMembershipRepo
    {
        Task<List<Membership>> GetMemberships();
        Task<bool> CreateMembership(Membership membership);
        Task<bool> UpdateMembership(Membership membership);
        Task<bool> DeleteMembership(Guid id);
        Task<Membership?> GetMembershipById(Guid id);
    }
	public class MembershipRepo : IMembershipRepo
	{
        private readonly FLBDbContext _context;
        public MembershipRepo(FLBDbContext context)
        {
            _context = context;
        }

		public async Task<bool> CreateMembership(Membership membership)
		{
			await _context.Memberships.AddAsync(membership);
            return await _context.SaveChangesAsync() > 0;
		}

		public async Task<bool> DeleteMembership(Guid id)
		{
			var membership = await _context.Memberships.FindAsync(id);
            if (membership == null)
            {
                return false;
            }
            _context.Memberships.Remove(membership);
            return await _context.SaveChangesAsync() > 0; 
		}

		public async Task<Membership?> GetMembershipById(Guid id)
		{
			return await _context.Memberships.FirstOrDefaultAsync(m => m.Id == id);
		}

		public async Task<List<Membership>> GetMemberships()
		{
			return await _context.Memberships.ToListAsync();
		}

		public async Task<bool> UpdateMembership(Membership membership)
		{
			_context.Memberships.Update(membership);
            return await _context.SaveChangesAsync() > 0;
		}
	}
}