using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.src.api.domains.Database;
using BE.src.api.domains.Model;
using Microsoft.EntityFrameworkCore;

namespace BE.src.api.repositories
{
    public interface ISocialProfileRepo
    {
        Task<List<SocialProfile>> GetSocialProfiles(Guid userId);
        Task<bool> AddNewSocialProfile(SocialProfile socialProfile);
        Task<bool> EditSocialProfile(SocialProfile socialProfile);
    }
	public class SocialProfileRepo : ISocialProfileRepo
	{
        private readonly FLBDbContext _context;
        public SocialProfileRepo(FLBDbContext context)
        {
            _context = context;
        }

		public async Task<bool> AddNewSocialProfile(SocialProfile socialProfile)
		{
			await _context.SocialProfiles.AddAsync(socialProfile);
            return await _context.SaveChangesAsync() > 0;
		}

		public async Task<bool> EditSocialProfile(SocialProfile socialProfile)
		{
			_context.SocialProfiles.Update(socialProfile);
            await _context.SaveChangesAsync();
            return true;
		}

		public async Task<List<SocialProfile>> GetSocialProfiles(Guid userId)
		{
			return await _context.SocialProfiles.Where(x => x.UserId == userId).ToListAsync();
		}
	}
}