using BE.src.api.domains.Database;
using BE.src.api.domains.DTOs.Membership;
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
		Task<bool> AddMembershipUser(MembershipUser membershipUser);
		Task<MembershipUser?> GetMembershipUserById(Guid membershipId, Guid userId);
		Task<MembershipUser?> GetMembershipUserRegistered(Guid userId);
		Task<bool> UpdateMembershipUser(MembershipUser membershipUser);
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
			return await _context.Memberships.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
		}

		public async Task<List<Membership>> GetMemberships()
		{
			return await _context.Memberships.AsNoTracking().ToListAsync();
		}

		public async Task<bool> UpdateMembership(Membership membership)
		{
			_context.Memberships.Update(membership);
            return await _context.SaveChangesAsync() > 0;
		}
		
		public async Task<bool> AddMembershipUser(MembershipUser membershipUser)
		{
			await _context.MemberUsers.AddAsync(membershipUser);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<MembershipUser?> GetMembershipUserById(Guid membershipId, Guid userId)
		{
			return await _context.MemberUsers.FirstOrDefaultAsync(mu => mu.MembershipId == membershipId && mu.UserId == userId);
		}

		public async Task<MembershipUser?> GetMembershipUserRegistered(Guid userId)
		{
			return await _context.MemberUsers.AsNoTracking().FirstOrDefaultAsync(mu => mu.UserId == userId);
		}

		public async Task<bool> UpdateMembershipUser(MembershipUser membershipUser)
		{
			_context.MemberUsers.Update(membershipUser);
			return await _context.SaveChangesAsync() > 0;
		}
	}
}