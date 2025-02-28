using BE.src.api.domains.DTOs.Membership;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using Microsoft.AspNetCore.Mvc;
using BE.src.api.shared.Type;

namespace BE.src.api.services
{
    public interface IMembershipServ
    {
        Task<IActionResult> GetMemberships();
        Task<IActionResult> CreateMembership(MembershipCreateDTO membership);
        Task<IActionResult> UpdateMembership(Guid id, MembershipUpdateDTO membership);
        Task<IActionResult> DeleteMembership(Guid id);
    }

	public class MembershipServ : IMembershipServ
	{
        private readonly IMembershipRepo _membershipRepo;
        private readonly ICacheService _cacheService;
        public MembershipServ(IMembershipRepo membershipRepo, ICacheService cacheService)
        {
            _membershipRepo = membershipRepo;
            _cacheService = cacheService;
        }
		public async Task<IActionResult> CreateMembership(MembershipCreateDTO membership)
		{
            try
            {
                var newMembership = new Membership
                {
                    Name = membership.Name,
                    Price = membership.Price,
                    ExpireTime = membership.ExpireTime,
                    Description = membership.Description,
                    CreateAt = DateTime.UtcNow,
                    UpdateAt = DateTime.UtcNow
                };
                var isCreated = await _membershipRepo.CreateMembership(newMembership);
                if (!isCreated)
                {
                    return ErrorResp.BadRequest("Fail to create membership");
                }
                return SuccessResp.Created("Membership created successfully");
            }
            catch (System.Exception ex)
            {
                return ErrorResp.BadRequest(ex.Message);
            }
		}

		public async Task<IActionResult> DeleteMembership(Guid id)
		{
			try
            {
                var isDeleted = await _membershipRepo.DeleteMembership(id);
                if (!isDeleted)
                {
                    return ErrorResp.BadRequest("Fail to delete membership");
                }
                return SuccessResp.Ok("Membership deleted successfully");
            }
            catch (System.Exception ex)
            {
                return ErrorResp.BadRequest(ex.Message);
            }
		}

		public async Task<IActionResult> GetMemberships()
		{
			
            try
            {
                var key = "memberships";
                var cachedMemberships = await _cacheService.Get<List<Membership>>(key);
                if (cachedMemberships != null)
                {
                    return SuccessResp.Ok(cachedMemberships);
                }

                var memberships = await _membershipRepo.GetMemberships();
                if (memberships.Count == 0)
                {
                    return ErrorResp.NotFound("No membership found");
                }

                await _cacheService.Set(key, memberships, TimeSpan.FromMinutes(10));

                return SuccessResp.Ok(memberships);
            }
            catch (System.Exception ex)
            {
                return ErrorResp.BadRequest(ex.Message);
            }
		}

		public async Task<IActionResult> UpdateMembership(Guid id, MembershipUpdateDTO membership)
		{
			try
            {
                var membershipFinding = await _membershipRepo.GetMembershipById(id);
                if (membershipFinding == null)
                {
                    return ErrorResp.NotFound("Membership not found");
                }

                membershipFinding.Name = membership.Name ?? membershipFinding.Name;
                membershipFinding.Price = membership.Price ?? membershipFinding.Price;
                membershipFinding.ExpireTime = membership.ExpireTime ?? membershipFinding.ExpireTime;
                membershipFinding.Description = membership.Description ?? membershipFinding.Description;
                membershipFinding.UpdateAt = DateTime.UtcNow;

                var isUpdated = await _membershipRepo.UpdateMembership(membershipFinding);
                if (!isUpdated)
                {
                    return ErrorResp.BadRequest("Fail to update membership");
                }
                return SuccessResp.Ok("Membership updated successfully");
            }
            catch (System.Exception ex)
            {
                return ErrorResp.BadRequest(ex.Message);
            }
		}
	}
}