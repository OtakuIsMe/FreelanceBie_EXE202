using BE.src.api.domains.DTOs.Membership;
using BE.src.api.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.controllers
{
	[ApiController]
	[Route("api/v1/membership")]
	public class MembershipController : ControllerBase
	{
		private readonly IMembershipServ _membershipServ;
		private readonly ILogger<MembershipController> _logger;
		public MembershipController(IMembershipServ membershipServ, ILogger<MembershipController> logger)
		{
			_membershipServ = membershipServ;
			_logger = logger;
		}
		[HttpGet("view")]
		public async Task<IActionResult> GetMemberships()
		{
			_logger.LogInformation("Get all memberships");
			return await _membershipServ.GetMemberships();
		}
		[Authorize(Policy = "Staff")]
		[HttpPost("create")]
		public async Task<IActionResult> CreateMembership([FromBody] MembershipCreateDTO membership)
		{
			_logger.LogInformation("Create membership");
			return await _membershipServ.CreateMembership(membership);
		}
		[Authorize(Policy = "Staff")]
		[HttpPut("update/{id}")]
		public async Task<IActionResult> UpdateMembership(Guid id, [FromForm] MembershipUpdateDTO membership)
		{
			_logger.LogInformation("Update membership");
			return await _membershipServ.UpdateMembership(id, membership);
		}
		[Authorize(Policy = "Staff")]
		[HttpDelete("delete/{id}")]
		public async Task<IActionResult> DeleteMembership(Guid id)
		{
			_logger.LogInformation("Delete membership");
			return await _membershipServ.DeleteMembership(id);
		}
	}
}
